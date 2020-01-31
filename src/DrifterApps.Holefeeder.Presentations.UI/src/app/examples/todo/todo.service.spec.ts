import { TestBed, inject, async } from '@angular/core/testing';

import { TodoService } from './todo.service';

describe('TodoService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({});
  });

  it('should be created',
    inject([TodoService], (service: TodoService) => {
      expect(service).toBeTruthy();
    })
  );

  it('should add item',
    async(
      inject([TodoService], (service: TodoService) => {
        service.addTodo({ prop: 'test' });
        service.todos$.subscribe(todos => expect(todos.length).toBe(1));
      })
    )
  );
});
