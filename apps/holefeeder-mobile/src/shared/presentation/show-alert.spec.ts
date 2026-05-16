import { Alert } from 'react-native';
import { showAlert } from './show-alert'; // Adjust the import path

const mockAlert = jest.spyOn(Alert, 'alert');

const mockT = jest.fn((key) => {
  const translations: { [key: string]: string } = {
    'alert.delete.title': 'Delete {{item}}',
    'alert.delete.message': 'Do you want to delete this {{item}}?',
    'alert.delete.cancelText': 'Cancel',
    'alert.delete.confirmText': 'Delete',
    'alert.discard.title': 'Discard Changes',
    'alert.discard.message': 'Are you sure you want to discard your changes?',
    'alert.discard.cancelText': 'Cancel',
    'alert.discard.confirmText': 'Discard',
    'alert.formError.title_one': 'Form Error',
    'alert.formError.title_other': 'Form Errors',
    'alert.formError.message_one': 'There is an error in the form. Please fix it before proceeding.',
    'alert.formError.message_other': 'There are errors in the form. Please fix them before proceeding.',
    'alert.formError.dismissText': 'Dismiss',
  };
  return translations[key] || '';
});

function findButton(text: string) {
  const alertArgs = mockAlert.mock.calls[0];
  const buttons = alertArgs[2];

  return buttons?.find((btn) => btn.text === text);
}

describe('showAlert', () => {
  afterEach(() => {
    mockAlert.mockClear();
  });

  it('returns an object with a showDeleteAlert, showDiscardAlert and showFormErrorAlert method', () => {
    const alertFunctions = showAlert(mockT);
    expect(typeof alertFunctions).toBe('object');
    expect(typeof alertFunctions.showDeleteAlert).toBe('function');
    expect(typeof alertFunctions.showDiscardAlert).toBe('function');
    expect(typeof alertFunctions.showFormErrorAlert).toBe('function');
  });

  describe('showDeleteAlert', () => {
    const item = 'delete-item';
    const onConfirm = jest.fn();
    const onCancel = jest.fn();
    let showDeleteAlert: ReturnType<typeof showAlert>['showDeleteAlert'];

    beforeEach(() => {
      ({ showDeleteAlert } = showAlert(mockT));
    });

    afterEach(() => {
      onConfirm.mockClear();
      onCancel.mockClear();
    });

    it('calls Alert.alert with the correct translated strings', () => {
      showDeleteAlert(item, { onConfirm, onCancel });

      expect(mockAlert).toHaveBeenCalledTimes(1);
      expect(mockAlert).toHaveBeenCalledWith(
        'Delete {{item}}',
        'Do you want to delete this {{item}}?',
        expect.arrayContaining([
          expect.objectContaining({
            text: 'Cancel',
            style: 'cancel',
            onPress: onCancel,
          }),
          expect.objectContaining({
            text: 'Delete',
            style: 'destructive',
            onPress: onConfirm,
          }),
        ])
      );
    });

    it('calls the onConfirm callback when the confirm button is pressed', () => {
      showDeleteAlert(item, { onConfirm, onCancel });

      findButton('Delete')?.onPress?.();

      expect(onConfirm).toHaveBeenCalledTimes(1);
      expect(onCancel).not.toHaveBeenCalled();
    });

    it('calls the onCancel callback when the cancel button is pressed', () => {
      showDeleteAlert(item, { onConfirm, onCancel });

      findButton('Cancel')?.onPress?.();

      expect(onCancel).toHaveBeenCalledTimes(1);
      expect(onConfirm).not.toHaveBeenCalled();
    });
  });

  describe('showDiscardAlert', () => {
    const onConfirm = jest.fn();
    const onCancel = jest.fn();
    let showDiscardAlert: ReturnType<typeof showAlert>['showDiscardAlert'];

    beforeEach(() => {
      ({ showDiscardAlert } = showAlert(mockT));
    });

    afterEach(() => {
      onConfirm.mockClear();
      onCancel.mockClear();
    });

    it('calls Alert.alert with the correct translated strings', () => {
      showDiscardAlert({ onConfirm, onCancel });

      expect(mockAlert).toHaveBeenCalledTimes(1);
      expect(mockAlert).toHaveBeenCalledWith(
        'Discard Changes',
        'Are you sure you want to discard your changes?',
        expect.arrayContaining([
          expect.objectContaining({
            text: 'Cancel',
            style: 'cancel',
            onPress: onCancel,
          }),
          expect.objectContaining({
            text: 'Discard',
            style: 'destructive',
            onPress: onConfirm,
          }),
        ])
      );
    });

    it('calls the onConfirm callback when the confirm button is pressed', () => {
      showDiscardAlert({ onConfirm, onCancel });

      findButton('Discard')?.onPress?.();

      expect(onConfirm).toHaveBeenCalledTimes(1);
      expect(onCancel).not.toHaveBeenCalled();
    });

    it('calls the onCancel callback when the cancel button is pressed', () => {
      showDiscardAlert({ onConfirm, onCancel });

      findButton('Cancel')?.onPress?.();

      expect(onCancel).toHaveBeenCalledTimes(1);
      expect(onConfirm).not.toHaveBeenCalled();
    });
  });
});
