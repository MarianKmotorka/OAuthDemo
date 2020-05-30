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
    if (!files[0]) return

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
      <input
        type='file'
        id='image-upload-input'
        style={{ display: 'none' }}
        onChange={uploadImage}
      />
      <button
        onClick={() => document.getElementById('image-upload-input').click()}
        style={{
          border: 'none',
          borderRadius: 6,
          background: 'orange',
          padding: 8,
          display: 'block'
        }}
      >
        Upload photo
      </button>
      <div>Email: {data.email}</div>
      <div>Name: {data.name}</div>
    </>
  )
}

export default MyInfo
