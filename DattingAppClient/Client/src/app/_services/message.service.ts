import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PaginatedResult } from '../_models/pagination';
import { setPaginatedResponse, setPaginationHeader } from './paginationHelper';
import { Message } from '../_models/message';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubsUrl;
  private http = inject(HttpClient);
  hubConnection? : HubConnection
  paingatedResult = signal<PaginatedResult<Message[]> | null>(null)
  messageThread = signal<Message[]>([])

  createHubConnection(user: User, otherUsername: string) {
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
      accessTokenFactory: () => user.token
    })
    .withAutomaticReconnect()
    .build()

    this.hubConnection.start().catch(error => console.log(error))

    this.hubConnection.on('ReceiveMessageThread', messages => {
      this.messageThread.set(messages)
    })

    this.hubConnection.on('NewMessage', message => {
      this.messageThread.update(messages=>[... messages,message])
    })
    
  }

  stopHubConnection() {
    if(this.hubConnection?.state === 'Connected') {
      this.hubConnection.stop().catch(error => console.log(error))
    }
  }

  getMessages(pageNumber: number, pageSize: number, container: string) {
    let params = setPaginationHeader(pageNumber, pageSize);

    params = params.append('Container', container);

    return this.http.get<Message[]>(this.baseUrl + 'messages', { observe: 'response', params })
      .subscribe({
        next: response => {
          setPaginatedResponse(response, this.paingatedResult);
        }
      });
  }

  getMessageThread(username: string) {
    return this.http.get<Message[]>(this.baseUrl + 'messages/thread/' + username);
  }

  async sendMessage(username: string, content: string) {
    return this.hubConnection?.invoke('SendMessage', { recipientUsername: username, content })
  }

  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
