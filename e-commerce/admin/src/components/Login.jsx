import axios from 'axios'
import React, { useState } from 'react'
import {  backendUrl } from '../App'
import { toast } from 'react-toastify'
import { FaEye, FaEyeSlash } from 'react-icons/fa';


export default function Login({setToken}) {
  console.log(`${backendUrl}/api/user/admin`, "backendURL")
    
    const [email, setEmail] = useState('')
    const [password, setPassword] = useState('')
    const [showPassword, setShowPassword] = useState(false)

    const onSubmitHandler = async (e) => {
        try {
            e.preventDefault();
            const response = await axios.post(`${backendUrl}/api/user/admin`, { email, password });
            console.log(backendUrl)
            
            if(response.data.success){
                setToken(response.data.token)
            }else {
                toast.error(response.data.message)
            }
        }catch (error){
            console.log(error);
            toast.error(error.message)
        }
    }
  return (
    <div className='min-h-screen flex items-center justify-center w-full'>
        <div className='bg-white shadow-md rounded-lg px-8 py-2 max-w-md'>
            <h1  className='text-2xl font-bold mb-4'>Admin Panel</h1>
            <form onSubmit={onSubmitHandler} >
                <div className='mb-3 min-w-72'>
                    <p className='text-sm font-medium text-gray-700 mb-2'>Email Address</p>
                    <input onChange={(e)=>setEmail(e.target.value)} value={email} className='rounded-md w-full px-3 py-2 border border-gray-300 outline-none' type='email' placeholder='your@email.com' required/>
                </div>


                <div className='mb-3 min-w-72'>
                <div className="relative">
        <input
          onChange={(e) => setPassword(e.target.value)}
          value={password}
          className='rounded-md w-full px-3 py-2 border border-gray-300 outline-none pr-10' // Espacio adicional para el botón
          type={showPassword ? 'text' : 'password'}
          placeholder='Enter your password'
          required
        />
        <button
          type='button'
          onClick={() => setShowPassword((prev) => !prev)}
          className="absolute right-2 top-1/2 transform -translate-y-1/2 text-black"
        >
          {showPassword ? <FaEye /> : <FaEyeSlash />}
        </button>
      </div>
                </div>

                
                <button className='mt-2 w-full py-2 px-4 rounded-md text-white bg-black' type='submit'>Login</button>
            </form>
        </div>
      
    </div>
  )
}
