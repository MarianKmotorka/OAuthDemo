import React, { useEffect } from 'react'
import { logout } from '../services/authService'
import { Redirect } from 'react-router-dom'

const Logout = () => {
  useEffect(() => {
    logout()
  }, [])

  return (
    <div>
      <Redirect
        to={{
          pathname: '/'
        }}
      />
    </div>
  )
}

export default Logout
