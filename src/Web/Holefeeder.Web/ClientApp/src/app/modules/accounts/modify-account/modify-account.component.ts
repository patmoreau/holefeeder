import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ActivatedRoute, Params, Router} from '@angular/router';
import {Observable, switchMap, tap} from 'rxjs';
import {Location} from '@angular/common';
import {ModifyAccountAdapter} from '../models/modify-account-command.model';
import {filterNullish, filterTrue} from '@app/shared';
import {AccountCommandsService} from '../services/account-commands.service';
import {AccountsService} from "@app/core";
import {CloseAccountAdapter} from "@app/modules/accounts/models/close-account-command.model";
import {ModalService} from "@app/core/modals/modal.service";

const accountIdParamName = 'accountId';

@Component({
  selector: 'app-modify-account',
  templateUrl: './modify-account.component.html',
  styleUrls: ['./modify-account.component.scss']
})
export class ModifyAccountComponent implements OnInit {

  @ViewChild('confirm', {static: true})
  confirmModalElement!: ElementRef;
  form!: FormGroup;

  accountId!: string;

  values$!: Observable<any>;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private location: Location,
    private accountsService: AccountsService,
    private commandsService: AccountCommandsService,
    private adapter: ModifyAccountAdapter,
    private closeAdapter: CloseAccountAdapter,
    private modalService: ModalService
  ) {
  }

  ngOnInit(): void {

    this.form = this.formBuilder.group({
      name: ['', Validators.required],
      type: [{value: '', disable: true}, Validators.required],
      openBalance: [0, [Validators.required, Validators.min(0)]],
      openDate: [{value: '', disable: true}, Validators.required],
      description: ['']
    });

    this.values$ = this.route.params.pipe(
      switchMap((params: Params) => this.accountsService.findById(params[accountIdParamName])),
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
      }));
  }

  onSubmit() {
    this.commandsService.modify(
      this.adapter.adapt(Object.assign({}, this.form.value, {
        id: this.accountId
      }))).subscribe(_ => this.router.navigate(['accounts', this.accountId]));
  }

  onDeactivate() {
    this.modalService.deactivate(
      'Are you sure you want to deactivate this account?'
    ).pipe(
      filterTrue(),
      switchMap(_ => this.commandsService.close(this.closeAdapter.adapt({id: this.accountId})))
    ).subscribe(_ => {
      this.router.navigate(['accounts']).then();
    });
  }

  goBack(): void {
    this.location.back();
  }
}
