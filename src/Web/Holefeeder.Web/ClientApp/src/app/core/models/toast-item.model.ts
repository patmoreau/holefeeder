import {Injectable} from "@angular/core";
import {Adapter} from "@app/shared/interfaces/adapter.interface";

export abstract class ToastItem {
  constructor(
    public message: string,
    public delay: number
  ) {
  }

  public abstract get className(): string;

  public abstract get type(): ToastType;
}

export class InfoToastItem extends ToastItem {
  constructor(
    message: string,
    delay: number = 5000
  ) {
    super(message, delay)
  }

  public get className(): string {
    return 'bg-info text-dark';
  }

  public get type(): ToastType {
    return ToastType.info;
  }
}

export class WarningToastItem extends ToastItem {
  constructor(
    message: string,
    delay: number = 5000
  ) {
    super(message, delay)
  }

  public get className(): string {
    return 'bg-warning text-dark';
  }

  public get type(): ToastType {
    return ToastType.warning;
  }
}

export class DangerToastItem extends ToastItem {
  constructor(
    message: string,
    delay: number = 5000
  ) {
    super(message, delay)
  }

  public get className(): string {
    return 'bg-danger text-light';
  }

  public get type(): ToastType {
    return ToastType.danger;
  }
}

export enum ToastType {
  info = 'info',
  warning = 'warning',
  danger = 'danger'
}

@Injectable({providedIn: "root"})
export class ToastItemAdapter implements Adapter<ToastItem> {
  adapt(item: any): ToastItem {
    if (item.type === ToastType.danger) {
      return new DangerToastItem(item.message, item.delay);
    } else if (item.type === ToastType.warning) {
      return new WarningToastItem(item.message, item.delay);
    } else {
      return new InfoToastItem(item.message, item.delay);
    }
  }
}
