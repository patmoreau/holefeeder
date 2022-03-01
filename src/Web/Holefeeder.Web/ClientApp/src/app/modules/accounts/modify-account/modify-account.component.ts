import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Params, Router } from '@angular/router';
import { ModalService } from '@app/shared/services/modal.service';
import { NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { Observable, switchMap, tap } from 'rxjs';
import { Location } from '@angular/common';
import { ModifyAccountAdapter } from '../models/modify-account-command.model';
import { filterNullish } from '@app/shared/rxjs.helper';
import { AccountCommandsService } from '../services/account-commands.service';
import { AccountsService } from '@app/core/services/accounts.service';

const accountIdParamName = 'accountId';

@Component({
  selector: 'app-modify-account',
  templateUrl: './modify-account.component.html',
  styleUrls: ['./modify-account.component.scss']
})
export class ModifyAccountComponent implements OnInit {

  @ViewChild('confirm', { static: true })
  confirmModalElement!: ElementRef;
  confirmModal!: NgbModalRef;
  confirmMessages!: string;

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
    private modalService: ModalService,
  ) {
  }

  ngOnInit(): void {

    this.form = this.formBuilder.group({
      name: ['', Validators.required],
      type: [{ value: '', disable: true }, Validators.required],
      openBalance: [0, [Validators.required, Validators.min(0)]],
      openDate: [{ value: '', disable: true }, Validators.required],
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
      }))).subscribe(id => this.router.navigate(['accounts', id]));
  }

  onDeactivate(content: any) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-deactivate-title' })
      .subscribe(_ => this.location.back());
  }

  goBack(): void {
    this.location.back();
  }
}
