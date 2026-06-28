import { useContext, useState } from 'react'
import {assets} from '../assets/assets'
import { Link, NavLink, useLocation} from 'react-router-dom'
import { ShopContext } from '../context/ShopContext';

export default function Navbar() {

  const [visible, setVisible] = useState(false);
  const {setShowSearch} = useContext(ShopContext);
  const location = useLocation();

  return (
    <div className="flex items-center justify-between py-5 font-medium">
       <Link to='/'> <img src={assets.logo} className='w-[249px] text-center' alt="" /> </Link> 

        <ul className='hidden sm:flex gap-5 text-sm text-gray-700'>
            <NavLink to='/' className='flex flex-col items-center gap-1'>
              <p>INICIO</p>
              <hr className='w-2/4 border-none h-[1.5px] bg-gray-700 hidden'/>
            </NavLink>

            <NavLink to='/collection' className='flex flex-col items-center gap-1'>
                <p>COLECCIÓN</p>
                <hr className='w-2/4 border-none h-[1.5px] bg-gray-700 hidden'/>
            </NavLink>

            <NavLink to='/about' className='flex flex-col items-center gap-1'>
                <p>SOBRE NOSOTROS</p>
                <hr className='w-2/4 border-none h-[1.5px] bg-gray-700 hidden'/>
            </NavLink>

            <NavLink to='/contact' className='flex flex-col items-center gap-1'>
                <p>CONTÁCTANOS</p>
                <hr className='w-2/4 border-none h-[1.5px] bg-gray-700 hidden'/>
            </NavLink>
         </ul>

         <div className='flex items-center gap-6'>
        {location.pathname === '/collection' && (
          <img
            onClick={() => setShowSearch(true)}
            src={assets.search_icon}
            className='w-5 cursor-pointer'
            alt=''
          />
        )}

       <button
          className="sm:hidden"
          onClick={() => setVisible((prev) => !prev)}
        >
          <img
            src={assets.dropdown_icon}
            className="w-6 cursor-pointer"
            alt="Menu"
          />
        </button>
      </div>

         {/** Sidebar menu for small screens */}
         <div className={`absolute top-0 bottom-0 overflow-hidden bg-white transition-all ${visible ? 'w-full' : 'w-0'}`}>
            <div className='flex flex-col text-gray-600'>
              <div onClick={()=> setVisible(false) } className='flex items-center gap-4 p-3 cursor-pointer'>
                <img src={assets.dropdown_icon} className='h-4 rotate-100' alt=''/>
                  <p>Regresar</p> 
              </div> 
              <NavLink onClick={()=>setVisible(false)} className='py-2 pl-6 border' to='/'>INICIO</NavLink>
              <NavLink onClick={()=>setVisible(false)} className='py-2 pl-6 border' to='/collection'>COLECCIÓN</NavLink>
              <NavLink onClick={()=>setVisible(false)} className='py-2 pl-6 border' to='/about'>SOBRE NOSOTROS</NavLink>
              <NavLink onClick={()=>setVisible(false)} className='py-2 pl-6 border' to='/contact'>CONTÁCTANOS</NavLink>
            </div>
         </div>
      
    </div>
  )
}
