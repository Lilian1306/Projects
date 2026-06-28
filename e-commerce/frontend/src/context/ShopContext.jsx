import { createContext, useEffect, useState } from "react";
import axios from 'axios'


export const ShopContext = createContext();

const ShopContextProvider = (props) => {
    
    const currency = 'Q';
    const delivery_fee = 10; 
    const backendUrl = import.meta.env.VITE_BACKEND_URL
    const [search, setSearch] = useState('')
    const [showSearch, setShowSearch] = useState(false)
    const [products,setProducts] = useState([])


    const getProductsData = async () => {
        try {
            const response = await axios.get(backendUrl + '/api/product/list' )
            if(response.data.success){
                setProducts(response.data.products)
            }else{
               toast.error(response.data.message)
            }

        }catch(error){
           console.log(error)
           toast.error(error.message)
        }
    }
    useEffect(() => {
        getProductsData()
    },[])
    
    const value = {
         products , currency, delivery_fee,
         search, setSearch, showSearch, setShowSearch,
         backendUrl
    }

   
    return (
        <ShopContext.Provider value={value}>
            {props.children}
        </ShopContext.Provider>
    )
}

export default ShopContextProvider;