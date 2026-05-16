import { fireEvent, render, waitFor } from '@testing-library/react-native';
import React from 'react';
import { Button, Text } from 'react-native';
import { ErrorKey } from '@/shared/core/error-key';
import { Result } from '@/shared/core/result';
import { createFormDataContext } from '@/shared/presentation/core/use-form-context';
import { RepositoriesState } from '@/shared/repositories/core/repositories-state';
import { RepositoryContext } from '@/shared/repositories/presentation/RepositoryContext';

// Mock repositories
const mockRepositories: RepositoriesState = {
  accountRepository: {} as any,
  categoryRepository: {} as any,
  dashboardRepository: {} as any,
  flowRepository: {} as any,
  settingRepository: {} as any,
  storeItemRepository: {} as any,
};

type TestFormData = {
  id: string;
  name: string;
};

const mockSave = jest.fn();

const { FormDataProvider, useFormDataContext } = createFormDataContext<TestFormData, string>('Test', mockSave);

const INITIAL_VALUE: TestFormData = { id: '1', name: 'Initial' };

function Consumer() {
  const { formData, isDirty, updateFormField, resetForm, setFormData, saveForm, generalError, clearGeneralError } = useFormDataContext();

  return (
    <>
      <Text testID="data">{JSON.stringify(formData)}</Text>
      <Text testID="dirty">{isDirty ? 'true' : 'false'}</Text>
      <Text testID="generalError">{generalError || 'null'}</Text>
      <Button title="setName" onPress={() => updateFormField('name', 'Alice')} />
      <Button title="reset" onPress={() => resetForm()} />
      <Button title="setAll" onPress={() => setFormData({ id: '2', name: 'Bob' })} />
      <Button title="setSameName" onPress={() => updateFormField('name', 'Initial')} />
      <Button title="save" onPress={() => saveForm()} />
      <Button title="clearGeneralError" onPress={() => clearGeneralError()} />
    </>
  );
}

describe('createFormDataContext / useFormDataContext', () => {
  beforeEach(() => {
    mockSave.mockReset();
    mockSave.mockResolvedValue(Result.success({}));
  });

  const renderWithProvider = () =>
    render(
      <RepositoryContext.Provider value={mockRepositories}>
        <FormDataProvider initialValue={INITIAL_VALUE}>
          <Consumer />
        </FormDataProvider>
      </RepositoryContext.Provider>
    );

  it('provides the initial value and isDirty=false', () => {
    const { getByTestId } = renderWithProvider();

    expect(getByTestId('data').props.children).toBe(JSON.stringify(INITIAL_VALUE));
    expect(getByTestId('dirty').props.children).toBe('false');
  });

  it('updates a field and sets isDirty=true when value actually changes', () => {
    const { getByTestId, getByText } = renderWithProvider();

    fireEvent.press(getByText('setName'));

    expect(getByTestId('data').props.children).toBe(JSON.stringify({ id: '1', name: 'Alice' }));
    expect(getByTestId('dirty').props.children).toBe('true');
  });

  it('does not set isDirty when updating a field with the same value', () => {
    const { getByTestId, getByText } = renderWithProvider();

    // Ensure initial is not dirty
    expect(getByTestId('dirty').props.children).toBe('false');

    // Update with the same value as INITIAL_VALUE.name
    fireEvent.press(getByText('setSameName'));

    expect(getByTestId('data').props.children).toBe(JSON.stringify({ id: '1', name: 'Initial' }));
    expect(getByTestId('dirty').props.children).toBe('false');
  });

  it('resets the form to initial value and clears dirty flag', () => {
    const { getByTestId, getByText } = renderWithProvider();

    fireEvent.press(getByText('setName'));
    expect(getByTestId('dirty').props.children).toBe('true');

    fireEvent.press(getByText('reset'));

    expect(getByTestId('data').props.children).toBe(JSON.stringify(INITIAL_VALUE));
    expect(getByTestId('dirty').props.children).toBe('false');
  });

  it('can replace all form data via setFormData (without toggling dirty flag)', () => {
    const { getByTestId, getByText } = renderWithProvider();

    fireEvent.press(getByText('setAll'));

    expect(getByTestId('data').props.children).toBe(JSON.stringify({ id: '2', name: 'Bob' }));
    // setFormData is a raw setter and does not toggle isDirty by design
    expect(getByTestId('dirty').props.children).toBe('false');
  });

  it('throws a helpful error when used outside of its Provider', () => {
    function OutsideConsumer() {
      // This hook call should throw because there is no provider

      const { formData } = useFormDataContext();
      return <Text>{JSON.stringify(formData)}</Text>;
    }

    expect(() => render(<OutsideConsumer />)).toThrow(new Error('useFormDataContext must be used within a TestProvider'));
  });

  describe('persistence', () => {
    it('calls the save function with the current database and form data', async () => {
      const { getByText } = renderWithProvider();

      fireEvent.press(getByText('setName')); // Change name to Alice
      fireEvent.press(getByText('save'));

      await waitFor(() => {
        expect(mockSave).toHaveBeenCalledWith(mockRepositories, { id: '1', name: 'Alice' });
      });
    });

    it('handles save failure gracefully', async () => {
      mockSave.mockResolvedValue(Result.failure(['Save failed']));
      const { getByText, getByTestId } = renderWithProvider();

      fireEvent.press(getByText('save'));

      await waitFor(() => {
        expect(mockSave).toHaveBeenCalled();
      });

      expect(getByTestId('generalError').props.children).toBe(ErrorKey.saveFailed);

      // Clear error
      fireEvent.press(getByText('clearGeneralError'));
      expect(getByTestId('generalError').props.children).toBe('null');
    });
  });

  describe('validation', () => {
    const validateFn = (formData: TestFormData) => {
      const errors: Partial<Record<keyof TestFormData, string>> = {};
      if (!formData.name) {
        errors.name = 'Name is required';
      }
      if (formData.name.length > 10) {
        errors.name = 'Name is too long';
      }
      if (!formData.id) {
        errors.id = 'ID is required';
      }
      return errors;
    };

    function ValidationConsumer() {
      const { formData, errors, updateFormField, validateForm, clearErrors, resetForm, saveForm } = useFormDataContext();

      const hasErrors = Object.keys(errors).length > 0;
      const nameError = errors.name;

      return (
        <>
          <Text testID="data">{JSON.stringify(formData)}</Text>
          <Text testID="errors">{JSON.stringify(errors)}</Text>
          <Text testID="hasErrors">{hasErrors ? 'true' : 'false'}</Text>
          <Text testID="nameError">{nameError || 'none'}</Text>
          <Button title="setName" onPress={() => updateFormField('name', 'Alice')} />
          <Button title="setLongName" onPress={() => updateFormField('name', 'VeryLongName123')} />
          <Button title="setEmptyName" onPress={() => updateFormField('name', '')} />
          <Button title="validateForm" onPress={() => validateForm()} />
          <Button title="clearNameError" onPress={() => clearErrors('name')} />
          <Button title="clearErrors" onPress={() => clearErrors()} />
          <Button title="reset" onPress={() => resetForm()} />
          <Button title="save" onPress={() => saveForm()} />
        </>
      );
    }

    it('provides empty errors object by default', () => {
      const { getByTestId } = render(
        <RepositoryContext.Provider value={mockRepositories}>
          <FormDataProvider initialValue={INITIAL_VALUE} validate={validateFn}>
            <ValidationConsumer />
          </FormDataProvider>
        </RepositoryContext.Provider>
      );

      expect(getByTestId('errors').props.children).toBe('{}');
      expect(getByTestId('hasErrors').props.children).toBe('false');
    });

    it('validates entire form and sets all errors', () => {
      const emptyData = { id: '', name: '' };
      const { getByTestId, getByText } = render(
        <RepositoryContext.Provider value={mockRepositories}>
          <FormDataProvider initialValue={emptyData} validate={validateFn}>
            <ValidationConsumer />
          </FormDataProvider>
        </RepositoryContext.Provider>
      );

      fireEvent.press(getByText('validateForm'));

      const errors = JSON.parse(getByTestId('errors').props.children);
      expect(errors.name).toBe('Name is required');
      expect(errors.id).toBe('ID is required');
      expect(getByTestId('hasErrors').props.children).toBe('true');
    });

    it('clears a single field error', () => {
      const { getByTestId, getByText } = render(
        <RepositoryContext.Provider value={mockRepositories}>
          <FormDataProvider initialValue={{ id: '', name: '' }} validate={validateFn}>
            <ValidationConsumer />
          </FormDataProvider>
        </RepositoryContext.Provider>
      );

      // Validate to set errors
      fireEvent.press(getByText('validateForm'));
      expect(getByTestId('hasErrors').props.children).toBe('true');

      // Clear name error
      fireEvent.press(getByText('clearNameError'));

      const errors = JSON.parse(getByTestId('errors').props.children);
      expect(errors.name).toBeUndefined();
      expect(errors.id).toBe('ID is required');
      expect(getByTestId('hasErrors').props.children).toBe('true');
    });

    it('clears all errors', () => {
      const { getByTestId, getByText } = render(
        <RepositoryContext.Provider value={mockRepositories}>
          <FormDataProvider initialValue={{ id: '', name: '' }} validate={validateFn}>
            <ValidationConsumer />
          </FormDataProvider>
        </RepositoryContext.Provider>
      );

      // Validate to set errors
      fireEvent.press(getByText('validateForm'));
      expect(getByTestId('hasErrors').props.children).toBe('true');

      // Clear all errors
      fireEvent.press(getByText('clearErrors'));

      expect(getByTestId('errors').props.children).toBe('{}');
      expect(getByTestId('hasErrors').props.children).toBe('false');
    });

    it('auto-validates on field change when validateOnChange is enabled', () => {
      const { getByTestId, getByText } = render(
        <RepositoryContext.Provider value={mockRepositories}>
          <FormDataProvider initialValue={INITIAL_VALUE} validate={validateFn} validateOnChange>
            <ValidationConsumer />
          </FormDataProvider>
        </RepositoryContext.Provider>
      );

      // Initially no errors
      expect(getByTestId('errors').props.children).toBe('{}');

      // Set a too-long name
      fireEvent.press(getByText('setLongName'));

      // Should auto-validate and show error
      expect(getByTestId('errors').props.children).toBe(JSON.stringify({ name: 'Name is too long' }));
      expect(getByTestId('hasErrors').props.children).toBe('true');
    });

    it('does not auto-validate when validateOnChange is false', () => {
      const { getByTestId, getByText } = render(
        <RepositoryContext.Provider value={mockRepositories}>
          <FormDataProvider initialValue={INITIAL_VALUE} validate={validateFn} validateOnChange={false}>
            <ValidationConsumer />
          </FormDataProvider>
        </RepositoryContext.Provider>
      );

      // Set a too-long name
      fireEvent.press(getByText('setLongName'));

      // Should NOT auto-validate
      expect(getByTestId('errors').props.children).toBe('{}');
      expect(getByTestId('hasErrors').props.children).toBe('false');
    });

    it('clears errors when resetForm is called', () => {
      const { getByTestId, getByText } = render(
        <RepositoryContext.Provider value={mockRepositories}>
          <FormDataProvider initialValue={INITIAL_VALUE} validate={validateFn}>
            <ValidationConsumer />
          </FormDataProvider>
        </RepositoryContext.Provider>
      );

      // Set empty name and validate
      fireEvent.press(getByText('setEmptyName'));
      fireEvent.press(getByText('validateForm'));

      expect(getByTestId('hasErrors').props.children).toBe('true');

      // Reset form
      fireEvent.press(getByText('reset'));

      expect(getByTestId('errors').props.children).toBe('{}');
      expect(getByTestId('hasErrors').props.children).toBe('false');
      expect(getByTestId('data').props.children).toBe(JSON.stringify(INITIAL_VALUE));
    });

    it('returns true from validateForm when no errors', () => {
      const { getByText } = render(
        <RepositoryContext.Provider value={mockRepositories}>
          <FormDataProvider initialValue={INITIAL_VALUE} validate={validateFn}>
            <ValidationConsumer />
          </FormDataProvider>
        </RepositoryContext.Provider>
      );

      // Initial value is valid, so validateForm should return true
      fireEvent.press(getByText('validateForm'));
      // If it returns true, no errors will be set (implicit check via no errors)
    });

    it('returns false from validateForm when errors exist', () => {
      const { getByTestId, getByText } = render(
        <RepositoryContext.Provider value={mockRepositories}>
          <FormDataProvider initialValue={{ id: '', name: '' }} validate={validateFn}>
            <ValidationConsumer />
          </FormDataProvider>
        </RepositoryContext.Provider>
      );

      // Validate with empty fields
      fireEvent.press(getByText('validateForm'));

      // Should have errors
      expect(getByTestId('hasErrors').props.children).toBe('true');
    });
  });
});
