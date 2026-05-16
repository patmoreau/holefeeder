import React, { createContext, ReactNode, useContext, useState } from 'react';
import { ErrorKey } from '@/shared/core/error-key';
import { Result } from '@/shared/core/result';
import { ErrorSheet } from '@/shared/presentation/components/ErrorSheet';
import { RepositoriesState } from '@/shared/repositories/core/repositories-state';
import { useRepositories } from '@/shared/repositories/core/use-repositories';

export type ValidationFunction<T, E extends string> = (formData: T) => Partial<Record<keyof T, E>>;

export type SaveFunction<T> = (repositories: RepositoriesState, formData: T) => Promise<Result<unknown>>;

export function createFormDataContext<T, E extends string>(displayName: string, save: SaveFunction<T>) {
  type FormContextType = {
    formData: T;
    isDirty: boolean;
    errors: Partial<Record<keyof T, E>>;
    updateFormField: <K extends keyof T>(field: K, value: T[K]) => void;
    resetForm: () => void;
    setFormData: React.Dispatch<React.SetStateAction<T>>;
    validateForm: () => boolean;
    saveForm: () => Promise<boolean>;
    clearErrors: (field?: keyof T) => void;
    generalError: ErrorKey | null;
    clearGeneralError: () => void;
  };

  type FormDataProviderProps = {
    initialValue: T;
    children: ReactNode;
    validate?: ValidationFunction<T, E>;
    validateOnChange?: boolean;
  };

  const FormContext = createContext<FormContextType | null>(null);
  FormContext.displayName = `${displayName}Context`;

  const FormDataProvider = ({ initialValue, children, validate, validateOnChange = false }: FormDataProviderProps) => {
    const [formData, setFormData] = useState<T>(initialValue);
    const [isDirty, setIsDirty] = useState(false);
    const [errors, setErrors] = useState<Partial<Record<keyof T, E>>>({});
    const [generalError, setGeneralError] = useState<ErrorKey | null>(null);
    const [showErrorSheet, setShowErrorSheet] = useState(false);
    const repositories = useRepositories();

    const updateFormField = <K extends keyof T>(field: K, value: T[K]) => {
      setFormData((prev) => {
        const newData = { ...prev, [field]: value };

        if (prev[field] !== value) {
          setIsDirty(true);
        }

        // Auto-validate only the changed field if enabled
        if (validateOnChange && validate) {
          const validationErrors = validate(newData);
          const fieldError = validationErrors[field];

          setErrors((prevErrors) => {
            if (fieldError) {
              return { ...prevErrors, [field]: fieldError };
            } else {
              const newErrors = { ...prevErrors };
              delete newErrors[field];
              return newErrors;
            }
          });
        }

        return newData;
      });
    };

    const validateForm = (): boolean => {
      if (!validate) return true;

      const validationErrors = validate(formData);
      setErrors(validationErrors);
      return Object.keys(validationErrors).length === 0;
    };

    const saveForm = async (): Promise<boolean> => {
      // Auto-validate before saving
      if (validate && !validateForm()) {
        return false;
      }

      const result = await save(repositories, formData);
      if (result.isFailure) {
        setGeneralError(ErrorKey.saveFailed);
        setShowErrorSheet(true);
        return false;
      }
      return true;
    };

    const clearGeneralError = () => {
      setGeneralError(null);
      setShowErrorSheet(false);
    };

    const clearErrors = (field?: keyof T) => {
      if (field) {
        setErrors((prev) => {
          const newErrors = { ...prev };
          delete newErrors[field];
          return newErrors;
        });
      } else {
        setErrors({});
      }
    };

    const resetForm = () => {
      setFormData(initialValue);
      setIsDirty(false);
      setErrors({});
    };

    const contextValue = {
      formData,
      isDirty,
      errors,
      updateFormField,
      resetForm,
      setFormData,
      validateForm,
      saveForm,
      clearErrors,
      generalError,
      clearGeneralError,
    };

    return (
      <FormContext.Provider value={contextValue}>
        {children}
        <ErrorSheet
          showError={showErrorSheet}
          setShowError={setShowErrorSheet}
          error={generalError || ErrorKey.saveFailed}
          onRetry={saveForm}
        />
      </FormContext.Provider>
    );
  };

  const useFormDataContext = (): FormContextType => {
    const context = useContext(FormContext);
    if (!context) {
      throw new Error(`useFormDataContext must be used within a ${displayName}Provider`);
    }
    return context;
  };

  return { FormDataProvider, useFormDataContext };
}
