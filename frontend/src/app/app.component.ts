import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule, ReactiveFormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  title = 'frontend';

  ws: WebSocket;
  messages: string[] = [];
  messageContent: FormControl = new FormControl('');
  userName: string = '';

  constructor() {
    this.ws = new WebSocket('ws://localhost:8181');
    this.ws.onmessage = (message) => {
      this.messages.push(message.data);
      console.log(this.messages);
    };

    // ขอให้ผู้ใช้ป้อนชื่อเมื่อเริ่มใช้งานด้วย Prompt
    this.setUserName();
  }

  setUserName(): void {
    this.userName = prompt('Please enter your name:') || 'Anonymous';
    this.ws.onopen = () => {
      this.ws.send(this.userName); // ส่งชื่อผู้ใช้ไปยังเซิร์ฟเวอร์
    };
  }

  sendMessage(): void {
    if (this.messageContent.value) {
      this.ws.send(this.messageContent.value);
      this.messageContent.reset();
    }
  }
}
