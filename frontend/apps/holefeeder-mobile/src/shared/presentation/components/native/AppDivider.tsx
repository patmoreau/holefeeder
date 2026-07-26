import { ExpoDivider, type ExpoDividerProps } from './expo/ExpoDivider';

export type AppDividerProps = ExpoDividerProps & {};

export const AppDivider = (props: AppDividerProps) => <ExpoDivider {...props} />;
