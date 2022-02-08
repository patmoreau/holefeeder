import { MessageAction } from "@app/shared/enums/message-action.enum";
import { MessageType } from "@app/shared/enums/message-type.enum";

export class Message {
  constructor(public type: MessageType, public action: MessageAction, public content?: any) { }

  static Empty = new Message(MessageType.general, MessageAction.noAction);
}
