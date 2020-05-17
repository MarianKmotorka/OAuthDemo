import React, { useEffect, useState } from 'react'
import { getToken } from '../services/authService'

const MyInfo = () => {
  const [data, setData] = useState(null)

  useEffect(() => {
    const fetchData = async () => {
      const response = await fetch('https://localhost:5001/api/auth/my-info', {
        method: 'GET',
        headers: {
          authorization: 'bearer ' + getToken()
        }
      })

      setData(await response.json())
    }

    fetchData()
  }, [setData])

  return !data ? (
    <div>Loading...</div>
  ) : (
    <>
      <img src={data.picture} style={{ maxWidth: '200px' }} alt='User' />
      <div>Email: {data.email}</div>
      <div>Name: {data.name}</div>
    </>
  )
}

export default MyInfo
