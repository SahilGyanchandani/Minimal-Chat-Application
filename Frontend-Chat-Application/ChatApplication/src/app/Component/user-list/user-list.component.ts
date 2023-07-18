import { Component } from '@angular/core';
import { LoginServiceService } from 'src/app/Services/login-service.service';
import { Message, MessageSend } from 'src/app/Models/message.model';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.css']
})
export class UserListComponent {
  Users: any;
  Msg: Message[] = []; // Initialize Msg as an empty array
  sendMsg: MessageSend[] = [];
  newMessage: string = ''; // Holds the message content from the textbox
  selectedUserId: string | null = null; // The receiver's userID
  editingMessageId: string | null = null; // The message ID being edited
  // Add the editedMessageContent property
  editedMessageContent: string = '';
   // Add properties for the context menu
   deletingMessageId: string | null = null;
   showContextMenu: boolean = false;
   contextMenuX: string = '0';
   contextMenuY: string = '0';
  

  constructor(private userService: LoginServiceService) {
    this.userService.onUserList().subscribe((data) => {
      this.Users = data;
    });
  }

  getUserConversation(user: any): void {
    this.userService.onMsgHistory(user.userID).subscribe((data: any) => {
      console.log('Data from API:', data);
      if (data && data.messages && Array.isArray(data.messages)) {
        this.Msg = data.messages; // Assign the array of messages to Msg
        this.selectedUserId = user.userID; // Set the selectedUserId to the receiver's userId
        console.log('Msg array:', this.Msg);

        // Reverse the Msg array to show the latest messages at the bottom
        this.Msg = this.Msg.reverse();
      } else {
        this.Msg = []; // If the data is not in the expected format, initialize as an empty array
        this.selectedUserId = null; // Reset selectedUserId
      }
    });
  }

  getUserById(userId: string): any {
    return this.Users.find((user: any) => user.userID === userId);
  }

  sendMessage(): void {
    if (this.newMessage.trim() === '' || !this.selectedUserId) {
      // Do not send an empty message or if there's no selected user
      return;
    }

    // Construct the message object to be sent to the backend
    const newMsg: MessageSend = {
      content: this.newMessage.trim(),
      receiverID: this.selectedUserId // Use selectedUserId as the recipient's receiverID
    };

    // Make a POST request to send the message
    this.userService.sendMessage(newMsg).subscribe(
      (response: any) => {
        // On successful response, prepend the sent message to the conversation
        this.Msg.unshift(response);

        // Clear the message input after sending
        this.newMessage = '';

        // Scroll to the bottom of the conversation
        this.scrollToBottom();
      },
      (error: any) => {
        // Handle error, display relevant error message to the user
        console.error('Error sending message:', error);
        alert('Failed to send message. Please try again.');
      }
    );
  }

  onMessageInput(event: Event): void {
    const target = event.target as HTMLTextAreaElement;
    this.newMessage = target.value;
  }

  editMessage(id: string): void {
    this.editingMessageId = id;
  }

  acceptEdit(id: string, editedContent: string): void {
    // Find the message with the given messageId
    const messageToUpdate = this.Msg.find((message) => message.id === id);

    if (!messageToUpdate) {
      console.error('Message not found for editing.');
      return;
    }

    // Make a PUT request to update the message content
    this.userService.updateMessage(id, editedContent).subscribe(
      (response: any) => {
        // On successful response, update the message in the conversation history
        messageToUpdate.content = editedContent;

        // Clear the editingMessageId after accepting the edit
        this.editingMessageId = null;
      },
      (error: any) => {
        // Handle error, display relevant error message to the user
        console.error('Error updating message:', error);
        alert('Failed to update message. Please try again.');
      }
    );
  }

  declineEdit(): void {
    // Clear the editingMessageId when declining the edit
    this.editingMessageId = null;
  }

  isMessageBeingEdited(messageId: string): boolean {
    return this.editingMessageId === messageId;
  }

  onMessageContextMenu(event: MouseEvent, message: Message): void {
    event.preventDefault(); // Prevent the default context menu from showing up
    // Show the context menu with options to edit and delete
    this.showContextMenu = true;
    this.contextMenuX = event.pageX + 'px';
    this.contextMenuY = event.pageY + 'px';
    // Set the deletingMessageId to the current message id
    this.deletingMessageId = message.id;
  }

  // Method to handle delete action
  onMessageDelete(message: Message): void {
    // Close the context menu
    this.hideContextMenu();

    // Show a confirmation dialog to the user
    const isConfirmed = confirm('Are you sure you want to delete this message?');
    if (isConfirmed) {
      // Make a DELETE request to delete the message
      this.userService.deleteMessage(message.id).subscribe(
        () => {
          // On successful response, remove the deleted message from the conversation
          this.removeMessage(message.id);
        },
        (error: any) => {
          // Handle error, display relevant error message to the user
          console.error('Error deleting message:', error);
          alert('Failed to delete message. Please try again.');
        }
      );
    }
  }

  // Method to remove a message from the conversation history
  private removeMessage(messageId: string): void {
    const messageIndex = this.Msg.findIndex((msg) => msg.id === messageId);
    if (messageIndex !== -1) {
      this.Msg.splice(messageIndex, 1);
    }
  }

  // Method to hide the context menu
  hideContextMenu(): void {
    this.showContextMenu = false;
    this.deletingMessageId = null;
  }

  private scrollToBottom(): void {
    setTimeout(() => {
      const messageContainer = document.querySelector('.message-container');
      if (messageContainer) {
        messageContainer.scrollTop = messageContainer.scrollHeight;
      }
    });
  }
}

