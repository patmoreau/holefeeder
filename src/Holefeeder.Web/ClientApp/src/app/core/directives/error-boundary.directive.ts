import { Directive, Input, TemplateRef, ViewContainerRef, OnInit, OnDestroy } from '@angular/core';
import { DebugService } from '../services/debug.service';

@Directive({
    selector: '[appErrorBoundary]',
    standalone: true
})
export class ErrorBoundaryDirective implements OnInit, OnDestroy {
    @Input() appErrorBoundary: string = 'Unknown component';
    @Input() errorTemplate?: TemplateRef<any>;

    constructor(
        private templateRef: TemplateRef<any>,
        private viewContainer: ViewContainerRef,
        private debugService: DebugService
    ) { }

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
        this.debugService.logError(error, {
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
