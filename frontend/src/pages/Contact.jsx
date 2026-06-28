import React from 'react'
import Title from '../components/Title'
import { assets } from '../assets/assets'

export default function Context() {
  const handleRedirect = (url) => {
    window.open(url, '_blank')
  }

  return (
    <div>
      <div className='text-center text-2xl pt-10 border-t'>
        <Title text1={'Nuestras'} text2={'Redes Sociales'}/>
      </div>

      <div className='my-10 flex flex-col justify-center md:flex-row gap-10 mb-20'>
       <img className='w-full md:max-w-[480px]' src={assets.contact_img} alt=''/>
       <div className='flex flex-col justify-center items-start gap-6'>
         <p className='font-semibold text-xl text-gray-600'>Nuestra Tienda</p>
         <p className='text-gray-500'>WEB SITE BOUTIQUE ✨, <br/>  Entrada, Comalapa</p>
         <p className="font-semibold text-xl text-gray-600">Nuestras Redes Sociales</p>
       <div className="flex flex-row items-center gap-4">
         <img 
           src={assets.facebook} 
           alt="Facebook" 
           className="w-12 h-12 cursor-pointer"
           onClick={() => handleRedirect('https://www.facebook.com')} />
         <img 
           src={assets.instagram} 
           alt="Instagram" 
           className="w-12 h-12 cursor-pointer"
           onClick={() => handleRedirect('https://www.instagram.com')} />
         <img 
           src={assets.tiktok} 
           alt="TikTok" 
           className="w-12 h-12 cursor-pointer"
           onClick={() => handleRedirect('https://www.tiktok.com')} />
       </div>
    </div>
       </div>
    </div>
  )
}
