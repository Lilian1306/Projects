import React from 'react'
import { assets } from '../assets/assets'
import { Link } from 'react-router-dom'

export default function Footer() {
  return (
    <div>
    <div className='flex flex-col sm:grid grid-cols-[3fr_1fr_1fr] gap-14 my-10 mt-40 text-sm'>
      <div>
       <Link to='/'  onClick={() => window.scrollTo({ top: 0, behavior: 'smooth' })}> <img src={assets.logo} alt='' className='mb-5 w-32'/> </Link> 
        <p className='w-full md:w-2/3 text-gray-600'>
        is simply dummy text of the printing and typesetting industry
        </p>
      </div>
      <div>
        <p className='text-xl font-medium mb-5'>Nuestra empresa</p>
        <ul className='flex flex-col gap-1 text-gray-600'>
         <Link to='/collection' onClick={() => window.scrollTo({ top: 0, behavior: 'smooth' })}> <li>Colección</li> </Link> 
           <Link to='/about' onClick={() => window.scrollTo({ top: 0, behavior: 'smooth' })}> <li>Sobre Nosotros</li> </Link>
        </ul>
        
      </div>
      
      <div>
        <p className='text-xl font-medium mb-5'>Contáctanos</p>
        <ul className='flex flex-col gap-1 text-gray-600'>
          <li>+502 7777-7777</li>
          <li>contact@website.com</li>
        </ul>
      </div>
    </div>

    <div>
       <hr/>
       <p className='py-5 text-sm text-center'>Copyright 2024@ website.com - All Right Reserved</p>
    </div>
   </div>
  )
}
