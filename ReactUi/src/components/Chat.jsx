import React, { useState, useEffect } from 'react'
import { map } from 'lodash'
import {
  HubConnectionBuilder,
  LogLevel,
  HttpTransportType
} from '@aspnet/signalr'

const Chat = () => {
  const [message, setMessage] = useState('')
  const [messages, setMessages] = useState([])
  const [nick, setNick] = useState('')
  const [hubConnection, setHubConnection] = useState(null)

  useEffect(() => {
    const nick = window.prompt('Your name:', 'John')
    setNick(nick)

    const connection = new HubConnectionBuilder()
      .withUrl('https://localhost:5001/api/chat', {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets
      })
      .configureLogging(LogLevel.Information)
      .build()

    connection.on('ReceiveMessage', (nick, receivedMessage) => {
      const text = `${nick}: ${receivedMessage}`
      setMessages(prev => [...prev, text])
    })

    setHubConnection(connection)

    connection
      .start()
      .then(() => console.log('Connection started!'))
      .catch(err => console.log('Error while establishing connection :('))
  }, [])

  const sendMessage = () => {
    hubConnection
      .invoke('SendMessage', nick, message)
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
