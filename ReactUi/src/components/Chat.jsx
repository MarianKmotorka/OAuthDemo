import React, { useState, useEffect } from 'react'
import { map } from 'lodash'
import {
  HubConnectionBuilder,
  LogLevel,
  HttpTransportType
} from '@aspnet/signalr'
import { getToken } from '../services/authService'

const Chat = () => {
  const [message, setMessage] = useState('')
  const [messages, setMessages] = useState([])
  const [hubConnection, setHubConnection] = useState(null)

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl('https://localhost:5001/api/chat', {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets,
        accessTokenFactory: getToken
      })
      .configureLogging(LogLevel.Information)
      .build()

    setHubConnection(connection)

    connection.on('ReceiveMessage', (nick, receivedMessage) => {
      const text = `${nick}: ${receivedMessage}`
      setMessages(prev => [...prev, text])
    })
  }, [])

  useEffect(() => {
    if (!hubConnection) return

    hubConnection.start().catch(console.log)
  }, [hubConnection])

  const sendMessage = () => {
    hubConnection
      .invoke('SendMessage', message)
      .catch(err => console.error(err))

    setMessage('')
  }

  return (
    <div>
      {map(messages, (x, i) => (
        <p key={i}>{x}</p>
      ))}
      <input
        type='text'
        value={message}
        style={{ background: 'green' }}
        onChange={e => setMessage(e.target.value)}
      />
      <button type='submit' onClick={sendMessage}>
        Send
      </button>
    </div>
  )
}

export default Chat
