import { HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

export interface ApiErrorBody {
  code: string;
  message: string;
  errors?: Record<string, string[]>;
}

export interface HttpErrorMessageOptions {
  conflictKey?: string;
  fallbackKey?: string;
}

const CODE_LIKE = /^[a-z][\w.]+$/i;

@Injectable({ providedIn: 'root' })
export class ApiErrorService {
  private readonly translate = inject(TranslateService);

  summary(error: ApiErrorBody): string {
    if (error.code === 'auth.banned') {
      return this.bannedMessage(error.message);
    }

    return this.translateCode(error.code);
  }

  fieldErrors(error: ApiErrorBody): Record<string, string[]> {
    const raw = error.errors ?? {};
    const translated: Record<string, string[]> = {};

    for (const [fieldKey, messages] of Object.entries(raw)) {
      translated[fieldKey] = messages.map((msg) =>
        this.translateFieldMessage(msg, fieldKey),
      );
    }

    return translated;
  }

  extractBody(error: unknown): ApiErrorBody | null {
    if (!(error instanceof HttpErrorResponse)) {
      return null;
    }

    const body = error.error as ApiErrorBody | null;
    return body?.code ? body : null;
  }

  messageForCode(code: string): string {
    return this.translateCode(code);
  }

  translateFieldMessage(raw: string, _fieldKey?: string): string {
    const trimmed = raw.trim();
    if (!trimmed) {
      return this.translate.instant('error.validation.failed');
    }

    if (CODE_LIKE.test(trimmed)) {
      return this.translateCode(trimmed.toLowerCase());
    }

    return this.translate.instant('error.validation.failed');
  }

  messageFromHttpError(
    error: unknown,
    options: HttpErrorMessageOptions = {},
  ): string {
    const body = this.extractBody(error);
    if (body) {
      return this.summary(body);
    }

    const hubCode = this.extractHubErrorCode(
      error instanceof Error ? error.message : String(error ?? ''),
    );
    if (hubCode) {
      return this.summary({ code: hubCode, message: hubCode });
    }

    const fallbackKey = options.fallbackKey ?? 'error.internal.error';

    if (
      error instanceof HttpErrorResponse &&
      error.status === 409 &&
      options.conflictKey
    ) {
      return this.translate.instant(options.conflictKey);
    }

    return this.translate.instant(fallbackKey);
  }

  formErrorsFromHttpError(error: unknown): Record<string, string[]> {
    const body = this.extractBody(error);
    return body ? this.fieldErrors(body) : {};
  }

  private extractHubErrorCode(message: string): string | null {
    const candidates = [
      message.trim(),
      ...message.split(':').map((part) => part.trim()),
    ].reverse();

    for (const candidate of candidates) {
      const code = candidate.replace(/\.$/, '');
      if (CODE_LIKE.test(code)) {
        return code.toLowerCase();
      }
    }

    return null;
  }

  private bannedMessage(apiMessage: string): string {
    const reasonPrefix = 'Your account has been banned: ';
    if (apiMessage.startsWith(reasonPrefix)) {
      const reason = apiMessage.slice(reasonPrefix.length).trim();
      if (reason.length > 0) {
        return this.translate.instant('error.auth.banned_with_reason', {
          reason,
        });
      }
    }

    return this.translate.instant('error.auth.banned');
  }

  private translateCode(code: string): string {
    const normalized = code.toLowerCase();
    const key = `error.${normalized}`;
    const translated = this.translate.instant(key);
    if (translated !== key) {
      return translated;
    }

    if (normalized === 'validation.failed') {
      return this.translate.instant('error.validation.failed');
    }

    return this.translate.instant('error.internal.error');
  }
}
