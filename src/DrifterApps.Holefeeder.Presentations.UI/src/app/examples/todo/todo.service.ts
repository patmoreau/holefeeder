import { Injectable } from '@angular/core';
import { Model, ModelFactory } from '@angular-extensions/model';
import { Observable } from 'rxjs';

const initialData: Todo[] = [];


@Injectable({
    providedIn: 'root'
})
export class TodoService {
  private model: Model<Todo[]>;

  todos$: Observable<Todo[]>;

  constructor(private modelFactory: ModelFactory<Todo[]>) {
    this.model = this.modelFactory.create(initialData);
    this.todos$ = this.model.data$;
  }

  addTodo(todo: Todo) {
    const todos = this.model.get();

    todos.push(todo);

    this.model.set(todos);
  }
}

export interface Todo {
  prop: string;
}
