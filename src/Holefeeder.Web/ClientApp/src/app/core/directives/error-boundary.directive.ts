import { Directive, Input, TemplateRef, ViewContainerRef, OnInit, OnDestroy, inject } from '@angular/core';
import { LoggerService } from '@app/core/logger/logger.service';

@Directive({
  selector: '[appErrorBoundary]',
  standalone: true
})
export class ErrorBoundaryDirective implements OnInit, OnDestroy {
  private templateRef = inject<TemplateRef<unknown>>(TemplateRef);
  private viewContainer = inject(ViewContainerRef);
  private loggerService = inject(LoggerService);

  @Input() appErrorBoundary: string = 'Unknown component';
  @Input() errorTemplate?: TemplateRef<unknown>;

  ngOnInit() {
    try {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } catch (error) {
      this.handleError(error as Error);
    }
  }

  ngOnDestroy() {
    try {
      this.viewContainer.clear();
    } catch (error) {
      this.handleError(error as Error);
    }
  }

  private handleError(error: Error) {
    this.loggerService.logError(error, {
      component: this.appErrorBoundary,
      action: 'template_render'
    });

    // Clear the view and optionally show error template
    this.viewContainer.clear();

    if (this.errorTemplate) {
      this.viewContainer.createEmbeddedView(this.errorTemplate);
    } else {
      // Create a simple error message
      const errorElement = document.createElement('div');
      errorElement.innerHTML = `
        <div style="padding: 10px; background: #f8d7da; color: #721c24; border: 1px solid #f5c6cb; border-radius: 4px; margin: 5px;">
          <strong>Error in ${this.appErrorBoundary}:</strong> ${error.message}
        </div>
      `;
      this.viewContainer.element.nativeElement.appendChild(errorElement);
    }
  }
}
