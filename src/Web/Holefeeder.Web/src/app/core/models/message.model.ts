import { MessageAction } from "./message-action.enum";
import { MessageType } from "./message-type.enum";

export class Message {
  constructor(public type: MessageType, public action: MessageAction, public content?: any) { }

  static Empty = new Message(MessageType.general, MessageAction.noAction);
}
