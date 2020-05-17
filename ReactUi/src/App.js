import React, { Component } from 'react'
import { Route, Switch } from 'react-router-dom'
import TopNavigation from './components/TopNavigation'
import Home from './components/Home'
import Login from './containers/Login.jsx'
import Logout from './containers/Logout.jsx'
import LoginCallback from './containers/LoginCallback'
import MyInfo from './containers/MyInfo'

const About = () => (
  <div>
    <h1>About</h1>
  </div>
)

const NotFound = () => (
  <div>
    <h1>NotFound</h1>
  </div>
)

class App extends Component {
  render() {
    return (
      <div>
        <TopNavigation />
        <main role='main' className='container'>
          <Switch>
            <Route exact path='/' component={Home} />
            <Route path='/about' component={About} />
            <Route path='/google-login-callback' component={LoginCallback} />
            <Route path='/login' component={Login} />
            <Route path='/logout' component={Logout} />
            <Route path='/myInfo' component={MyInfo} />
            <Route component={NotFound} />
          </Switch>
        </main>
      </div>
    )
  }
}

export default App
