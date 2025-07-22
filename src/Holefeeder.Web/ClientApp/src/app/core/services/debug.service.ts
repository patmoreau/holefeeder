import { Injectable, isDevMode } from '@angular/core';

interface ErrorContext {
  component?: string;
  action?: string;
  userAgent?: string;
  url?: string;
  timestamp?: Date;
  userId?: string;
}

@Injectable({
  providedIn: 'root'
})
export class DebugService {
  private errorHistory: Array<{ error: Error; context?: ErrorContext }> = [];
  private maxHistorySize = 50;

  constructor() {
    // In development mode, expose debug service to window for console access
    if (isDevMode()) {
      (window as any).debugService = this;
      console.log('🔧 DebugService available at window.debugService');
    }
  }

  logError(error: Error, context?: ErrorContext) {
    const errorEntry = {
      error,
      context: {
        ...context,
        userAgent: navigator.userAgent,
        url: window.location.href,
        timestamp: new Date(),
      }
    };

    this.errorHistory.unshift(errorEntry);

    // Keep only the most recent errors
    if (this.errorHistory.length > this.maxHistorySize) {
      this.errorHistory.splice(this.maxHistorySize);
    }

    if (isDevMode()) {
      console.group(`🐛 Error logged by DebugService`);
      console.error('Error:', error);
      console.log('Context:', errorEntry.context);
      console.groupEnd();
    }
  }

  getErrorHistory() {
    return [...this.errorHistory];
  }

  clearErrorHistory() {
    this.errorHistory = [];
    if (isDevMode()) {
      console.log('🧹 Error history cleared');
    }
  }

  exportErrorReport(): string {
    const report = {
      timestamp: new Date().toISOString(),
      userAgent: navigator.userAgent,
      url: window.location.href,
      errors: this.errorHistory.map(entry => ({
        message: entry.error.message,
        stack: entry.error.stack,
        context: entry.context
      }))
    };

    return JSON.stringify(report, null, 2);
  }

  downloadErrorReport() {
    const report = this.exportErrorReport();
    const blob = new Blob([report], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `error-report-${new Date().toISOString().split('T')[0]}.json`;
    link.click();
    URL.revokeObjectURL(url);
  }

  // Helper methods for console debugging
  logInfo(message: string, data?: any) {
    if (isDevMode()) {
      console.log(`ℹ️ ${message}`, data);
    }
  }

  logWarning(message: string, data?: any) {
    if (isDevMode()) {
      console.warn(`⚠️ ${message}`, data);
    }
  }

  logSuccess(message: string, data?: any) {
    if (isDevMode()) {
      console.log(`✅ ${message}`, data);
    }
  }
}
