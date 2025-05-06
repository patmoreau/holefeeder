import { CommonModule, Location } from '@angular/common';
import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { AccountsService, ModalService } from '@app/core/services';
import { AccountEditComponent } from '@app/modules/accounts/account-edit/account-edit.component';
import { CloseAccountAdapter } from '@app/modules/accounts/models/close-account-command.model';
import { LoaderComponent } from '@app/shared/components';
import { filterNullish, filterTrue } from '@app/shared/helpers';
import { Observable, switchMap, tap } from 'rxjs';
import { ModifyAccountCommand } from '../models/modify-account-command.model';
import { AccountCommandsService } from '../services/account-commands.service';
import { Account } from '@app/shared/models';

const accountIdParamName = 'accountId';

@Component({
    selector: 'app-modify-account',
    templateUrl: './modify-account.component.html',
    styleUrls: ['./modify-account.component.scss'],
    imports: [
        CommonModule,
        ReactiveFormsModule,
        LoaderComponent,
        AccountEditComponent,
    ]
})
export class ModifyAccountComponent implements OnInit {
  @ViewChild('confirm', { static: true })
  confirmModalElement!: ElementRef;
  form!: FormGroup;

  accountId!: string;

  values$!: Observable<Account>;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private location: Location,
    private accountsService: AccountsService,
    private commandsService: AccountCommandsService,
    private closeAdapter: CloseAccountAdapter,
    private modalService: ModalService
  ) {}

  ngOnInit(): void {
    this.form = this.formBuilder.group({
      name: ['', Validators.required],
      type: [{ value: '', disable: true }, Validators.required],
      openBalance: [0, [Validators.required, Validators.min(0)]],
      openDate: [{ value: '', disable: true }, Validators.required],
      description: [''],
    });

    this.values$ = this.route.params.pipe(
      switchMap((params: Params) =>
        this.accountsService.findById(params[accountIdParamName])
      ),
      filterNullish(),
      tap(account => {
        this.accountId = account.id;
        this.form.patchValue({
          name: account.name,
          type: account.type,
          openBalance: account.openBalance,
          openDate: account.openDate,
          description: account.description,
        });
      })
    );
  }

  onSubmit() {
    this.commandsService
      .modify(
        ModifyAccountCommand.fromObject(
          Object.assign({}, this.form.value, {
            id: this.accountId,
          })
        )
      )
      .subscribe(() => this.router.navigate(['accounts', this.accountId]));
  }

  onDeactivate() {
    this.modalService
      .deactivate('Are you sure you want to deactivate this account?')
      .pipe(
        filterTrue(),
        switchMap(() =>
          this.commandsService.close(
            this.closeAdapter.adapt({ id: this.accountId })
          )
        )
      )
      .subscribe(() => {
        this.router.navigate(['accounts']).then();
      });
  }

  goBack(): void {
    this.location.back();
  }
}
