import React, { useEffect } from 'react'
import { useLocation, useHistory } from 'react-router-dom'
import queryString from 'query-string'
import config from '../config.json'
import { login } from '../services/authService'

const LoginCallback = () => {
  const { search } = useLocation()
  const history = useHistory()
  const { code } = queryString.parse(search)

  useEffect(() => {
    const sendCodeToServer = async () => {
      const response = await fetch(
        `${config.SERVER_AUTH_CALLBACK_URL}?code=${code}`
      )

      const { token } = await response.json()
      login(token)
      history.replace('/myInfo')
    }

    sendCodeToServer()
  }, [code, history])

  return <div>Authenticating...</div>
}

export default LoginCallback
