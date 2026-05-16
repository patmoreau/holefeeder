import { act, fireEvent, render, screen } from '@testing-library/react-native';
import React from 'react';
import { ErrorKey } from '@/shared/core/error-key';
import { ErrorSheet } from '@/shared/presentation/components/ErrorSheet';
import { aLightThemeState } from '@/shared/theme/__tests__/theme-state-for-test';
import { ThemeProviderForTest } from '@/shared/theme/__tests__/ThemeProviderForTest';

xdescribe('ErrorSheet', () => {
  const themeState = aLightThemeState();

  it('renders nothing when showError is false', () => {
    render(
      <ThemeProviderForTest overrides={themeState}>
        <ErrorSheet showError={false} setShowError={jest.fn()} error={ErrorKey.noInternetConnection} />
      </ThemeProviderForTest>
    );

    expect(screen.queryByText('errors.noInternetConnection.title')).not.toBeOnTheScreen();
  });

  it('renders the bottom sheet when showError is true', () => {
    render(
      <ThemeProviderForTest overrides={themeState}>
        <ErrorSheet showError={true} setShowError={jest.fn()} error={ErrorKey.noInternetConnection} />
      </ThemeProviderForTest>
    );

    expect(screen.queryByText('errors.noInternetConnection.title')).toBeOnTheScreen();
  });

  it('displays the error title and message', () => {
    render(
      <ThemeProviderForTest overrides={themeState}>
        <ErrorSheet showError={true} setShowError={jest.fn()} error={ErrorKey.noInternetConnection} />
      </ThemeProviderForTest>
    );

    expect(screen.queryByText('errors.noInternetConnection.title')).toBeOnTheScreen();
    expect(screen.queryByText('errors.noInternetConnection.message')).toBeOnTheScreen();
  });

  it('calls setShowError when dismiss button is pressed', () => {
    const setShowError = jest.fn();
    render(
      <ThemeProviderForTest overrides={themeState}>
        <ErrorSheet showError={true} setShowError={setShowError} error={ErrorKey.noInternetConnection} />
      </ThemeProviderForTest>
    );

    const button = screen.queryByRole('button', { name: 'errorSheet.dismiss' });

    act(() => fireEvent.press(button));
    expect(setShowError).toHaveBeenCalledWith(false);
  });

  it('shows retry button when onRetry is provided', () => {
    const onRetry = jest.fn();
    render(
      <ThemeProviderForTest overrides={themeState}>
        <ErrorSheet showError={true} setShowError={jest.fn()} error={ErrorKey.noInternetConnection} onRetry={onRetry} />
      </ThemeProviderForTest>
    );

    const button = screen.queryByRole('button', { name: 'errorSheet.retry' });

    expect(button).toBeOnTheScreen();
  });

  it('calls onRetry when retry button is pressed', () => {
    const onRetry = jest.fn();
    render(
      <ThemeProviderForTest overrides={themeState}>
        <ErrorSheet showError={true} setShowError={jest.fn()} error={ErrorKey.noInternetConnection} onRetry={onRetry} />
      </ThemeProviderForTest>
    );

    const button = screen.getByRole('button', { name: 'errorSheet.retry' });

    act(() => fireEvent.press(button));
    expect(onRetry).toHaveBeenCalled();
  });

  it('does not show retry button when onRetry is not provided', () => {
    render(
      <ThemeProviderForTest overrides={themeState}>
        <ErrorSheet showError={true} setShowError={jest.fn()} error={ErrorKey.noInternetConnection} />
      </ThemeProviderForTest>
    );

    const button = screen.queryByRole('button', { name: 'errorSheet.retry' });

    expect(button).not.toBeOnTheScreen();
  });
});
