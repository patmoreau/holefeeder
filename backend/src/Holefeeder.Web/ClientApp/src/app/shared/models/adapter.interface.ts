export interface Adapter<T> {
  adapt(item: unknown): T;
}
