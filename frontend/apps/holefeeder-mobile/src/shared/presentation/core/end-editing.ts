import ExpoEndEditing from '@/modules/expo-end-editing';

const nextFrame = () => new Promise<void>((resolve) => requestAnimationFrame(() => resolve()));

export const endEditing = async (): Promise<void> => {
  await ExpoEndEditing.endEditing();
  await nextFrame();
};
