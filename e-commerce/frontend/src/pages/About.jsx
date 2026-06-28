import React from 'react'
import Title from '../components/Title'
import { assets } from '../assets/assets'

export default function About() {
  return (
    <div>
      <div className='text-2xl text-center pt-8 border-t'>
        <Title text1={'Sobre'} text2={'Nosotros'}/>
      </div>
      <div className='my-10 flex flex-col md:flex-row gap-16'>
        <img className='w-full md:max-w-[450px]' src={assets.about_img} alt=''/>
         <div className='flex flex-col justify-center gap-6 md:w-2/4 text-gray-600'>
            <p>Lorem Ipsum is simply dummy text of the printing and typesetting industry. </p>
            <p>Lorem Ipsum is simply dummy text of the printing and typesetting industry. </p>
            <b className='text-gray-800'>Nuestra Misión</b>
            <p>Lorem Ipsum is simply dummy text of the printing and typesetting industry.</p>
         </div>
      </div>
      <div className='text-xl py-4'>
        <Title text1={'¿Por qué '} text2={'elegirnos?'}/>
      </div>
      <div className='flex flex-col md:flex-row text-sm mb-20'>
       <div className='border px-10 md:px-16 py-8 sm:py-20 flex flex-col gap-5'>
        <b>Quality Assurance</b>
        <p className='text-gray-600'>Lorem Ipsum is simply dummy text of the printing and typesetting industry.</p>
       </div>
       <div className='border px-10 md:px-16 py-8 sm:py-20 flex flex-col gap-5'>
        <b>Convenience</b>
        <p className='text-gray-600'>Lorem Ipsum is simply dummy text of the printing and typesetting industry.</p>
       </div>
       <div className='border px-10 md:px-16 py-8 sm:py-20 flex flex-col gap-5'>
        <b>Exeptional Customer Service</b>
        <p className='text-gray-600'>Lorem Ipsum is simply dummy text of the printing and typesetting industry.</p>
       </div>
      </div>
     
    </div>
  )
}
