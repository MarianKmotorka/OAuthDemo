import React from 'react'
import { getToken } from '../services/authService'
import { withRouter, NavLink } from 'react-router-dom'

const TopNavigation = () => {
  const isLoggedIn = !!getToken()

  let loginLink = '/login'
  let loginText = 'Login'

  if (isLoggedIn) {
    loginLink = '/logout'
    loginText = 'Logout'
  } else {
    loginLink = '/login'
    loginText = 'Login'
  }
  return (
    <div>
      <nav className='navbar navbar-expand-md navbar-dark fixed-top bg-dark'>
        <a className='navbar-brand' href='/'>
          Zigot App
        </a>
        <div className='collapse navbar-collapse' id='navbarsExampleDefault'>
          <ul className='navbar-nav mr-auto'>
            <li className='nav-item'>
              <NavLink exact className='nav-link' to='/'>
                Fene{' '}
              </NavLink>
            </li>
            <li className='nav-item'>
              <NavLink exact className='nav-link' to='/about'>
                Bing{' '}
              </NavLink>
            </li>
            <li className='nav-item'>
              <NavLink exact className='nav-link' to={loginLink}>
                {loginText}{' '}
              </NavLink>
            </li>
            {isLoggedIn && (
              <li className='nav-item'>
                <NavLink exact className='nav-link' to='/myInfo'>
                  My info
                </NavLink>
              </li>
            )}
          </ul>
        </div>
      </nav>
    </div>
  )
}

export default withRouter(TopNavigation)
