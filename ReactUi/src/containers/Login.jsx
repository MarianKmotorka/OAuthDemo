import React from 'react'
import config from '../config.json'

const Login = () => {
  const onGoogleLoginClick = () => {
    const queryParams = [
      `client_id=${config.GOOGLE_CLIENT_ID}`,
      `redirect_uri=${config.GOOGLE_AUTH_CALLBACK_URL}`,
      'response_type=code',
      'scope=openid profile email',
      'access_type=offline',
      'state=myCustomState'
    ].join('&')

    const url = 'https://accounts.google.com/o/oauth2/v2/auth?' + queryParams

    window.location = url
  }

  return (
    <div>
      <h1>Login</h1>
      <div>
        <button onClick={onGoogleLoginClick}>GOOGLE LOGIN</button>
      </div>
    </div>
  )
}

export default Login
