export const logout = () => localStorage.removeItem('token')

export const login = token => localStorage.setItem('token', token)

export const getToken = () => localStorage.getItem('token') || null
