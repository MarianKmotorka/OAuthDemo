import React, { useEffect, useState } from 'react'
import { getToken } from '../services/authService'

const MyInfo = () => {
  const [data, setData] = useState(null)

  const fetchProfile = async () => {
    const response = await fetch('https://localhost:5001/api/profile', {
      method: 'GET',
      headers: {
        authorization: 'bearer ' + getToken()
      }
    })

    setData(await response.json())
  }

  useEffect(() => {
    fetchProfile()
  }, [])

  const uploadImage = async ({ target: { files } }) => {
    const data = new FormData()
    data.append('image', files[0])

    await fetch('https://localhost:5001/api/profile/image', {
      method: 'POST',
      headers: {
        authorization: 'bearer ' + getToken()
      },
      body: data
    })

    fetchProfile()
  }

  return !data ? (
    <div>Loading...</div>
  ) : (
    <>
      <img
        style={{
          objectFit: 'cover',
          width: 200,
          height: 200,
          borderRadius: '50%'
        }}
        src={data.picture}
        alt='User'
      />
      <input type='file' style={{ display: 'block' }} onChange={uploadImage} />
      <div>Email: {data.email}</div>
      <div>Name: {data.name}</div>
    </>
  )
}

export default MyInfo
