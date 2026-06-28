
import { useContext, useEffect, useState } from 'react'
import {useParams} from 'react-router-dom'
import { ShopContext } from '../context/ShopContext'
import { assets } from '../assets/assets';
import RelatedProducts from '../components/RelatedProducts';

export default function Product() {

  const { productId } = useParams()
  const { products, currency } = useContext(ShopContext);
  const [ productData, setProductData] = useState(false)
  const [ image, setImage ] = useState('')
  const [ size, setSize ] =useState('')
  const [ isZoomed, setIsZoomed] = useState(false);
  const [ transform, setTransform] = useState('center');

  const fetchProductData = async () => {
     products.map((item) => {
      if(item._id === productId){
        setProductData(item)
        setImage(item.image[0])
        console.log(item)
        return null; 
      }
     })
  }

  useEffect(() => {
    fetchProductData()
  },[productId, products])
   
  return productData ?  (
    <div className='border-t-2 pt-10 transition-opacity ease-in duration-500 opacity-100'>
      {/** Products Data */}
      <div className='flex gap-12 sm:gap-12 flex-col sm:flex-row'> 
        
        {/** Prdocuts Images */}
        <div className='flex-1 flex flex-col-reverse gap-3 sm:flex-row'>
        <div className='flex sm:flex-col overflow-x-auto sm:overflow-y-scroll justify-between sm:justify-normal sm:w-[18.7%] w-full'>
           {
            productData.image.map((item, index) => (
              <img onClick={()=>setImage(item)} className='w-[24%] sm:w-full sm:mb-3 flex-shrink-0 cursor-pointer' src={item} key={index} alt='' />
            ))
           }
         </div>

     <div
      className="w-full sm:w-[80%] overflow-hidden"
      onMouseMove={(e) => {
        if (!isZoomed) return;

        const rect = e.currentTarget.getBoundingClientRect();
        const x = ((e.clientX - rect.left) / rect.width) * 100; 
        const y = ((e.clientY - rect.top) / rect.height) * 100; 

        setTransform(`${x}% ${y}%`); 
      }}
      onClick={() => setIsZoomed(!isZoomed)} 
    >
      <img
        src={image}
        alt=""
        className={`w-full h-auto transition-transform duration-300 ${
          isZoomed ? 'scale-150 cursor-move' : 'scale-100 cursor-pointer'
        }`}
        style={{
          transformOrigin: transform
        }}
      />
    </div>
        </div>

        {/**  PRODUCTS INFORMATION  */}
        <div className='flex-1'>
          <h1 className='flex-1 font-medium text-2xl mt-2'>{productData.name}</h1>
           <div className='flex items-center gap-1 mt-2'>
            <img src={assets.star_icon} alt='' className='w-3 5' />
            <img src={assets.star_icon} alt='' className='w-3 5' />
            <img src={assets.star_icon} alt='' className='w-3 5' />
            <img src={assets.star_icon} alt='' className='w-3 5' />
            <img src={assets.star_dull_icon} alt='' className='w-3 5' />
            <p className='pl-2'>(122)</p>
         </div>
         <p className='mt-5 text-3xl font-medium'>{currency}{productData.price}</p>
         <p className='mt-5 text-gray-500 md:w-4/5'>{productData.description}</p>
         <div className='flex flex-col gap-4 my-8'>
           <p>Tallas disponibles</p>
           <div className='flex gap-2'>
             {productData.sizes.map((item, index)=>(
              <button onClick={()=>setSize(item)} className={`border py-2 px-4 bg-gray-100 ${item === size ? 'bg-purple-500' : ''  }`} key={index}>{item}</button>
             ))}
           </div>
         </div>
         <hr className='mt-8 sm:w-4/5'/>
         <div className='text-sm text-gray-500 mt-5 flex flex-col gap-1 '>
          <p>Producto 100% Original</p>
         </div>
        </div>
      </div>

      {/** Display related products */}
      <RelatedProducts category={productData.category} subCategory={productData.subCategory}/>
      
    </div>
  ) : <div className='opacity-0'></div>
}
