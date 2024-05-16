import { MessageAction, MessageType } from '@app/shared/models';

export class Message {
  static Empty = new Message(MessageType.general, MessageAction.noAction);

  constructor(
    public type: MessageType,
    public action: MessageAction,
    public content?: unknown
  ) {}
}
