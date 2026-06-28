import React, { useEffect, useState } from "react"
import Card from "./Card"
import { Member } from "../data/data"
import Error from "./Error"
import { IoMdArrowRoundBack } from "react-icons/io"
import { downloadImage } from "../utils/downloadImage"
import { FcDownload } from "react-icons/fc"


export default function Formulario() {

  const [name, setName] = useState('')
  const [country, setCountry] = useState('')
  const [cards, setCards] = useState('')
  const [error, setError] = useState('')
  const [enviar, setEnviar] = useState<any>(() => {
  const save = localStorage.getItem('credenciales')
    if(save) {
      return JSON.parse(save)
    }
    return null
  })

  useEffect(() => {
    localStorage.setItem('credenciales', JSON.stringify(enviar))
  },[enviar])


   const handleSubmit = (e: React.FormEvent) => {
     e.preventDefault()
     
     if(name === '' || country === '' || cards === '') {
      setError('Todos los campos son obligatorios')
      return 
     }

     setError('')

     const memberFound = Member.find((m) => m.name === cards)
     setEnviar({name, country, cards, image: memberFound ? memberFound?.image : ""})

     setName('')
     setCountry('')
     setCards('')
  }

  return (
    <div className=" mt-20">
        {!enviar ? (    
        <form 
          className="items-center mt-10 flex flex-col bg-purple-500 shadow-xl rounded-lg p-10 w-96 mx-auto"
          onSubmit={handleSubmit}
        >

          {error && <Error>{error}</Error>}
        
          <label htmlFor="name" className=" text-white"
          >Escriba tu nombre: </label>
          <input
           type="text"
           id="name"
           placeholder="Escribe tu nombre aqui"
           className="border border-gray-900 rounded-lg p-2 w-full "
           value={name}
           onChange={(e) => setName(e.target.value)}
          />

          <label htmlFor="country" className="text-white mt-5"
          >Escribe tu ciudad:</label>
          <input
            type="text"
            id="country"
            placeholder="Escribe tu ciudad aqui"
            className="border border-gray-900 rounded-lg p-2 w-full"
            value={country} 
            onChange={(e) => setCountry(e.target.value)}
          />

        <div>
        
          <select
             id="cards"
             className="border border-gray-900 rounded-lg w-60 mt-5 p-2 text-black"
             value={cards}
             onChange={(e) => setCards(e.target.value)}
          >
            <option value='' disabled> -- Selecciona Una Opcion -- </option>
            <option value="Kim Namjoon" className="text-center" >Kim NamJoon</option>
            <option value="Kim SeokJin" className="text-center"  >Kim SeokJin</option>
            <option value="Min Yoongi" className="text-center" >Min Yoongi</option>
            <option value="Jung Hoseok" className="text-center" >Jung Hoseok</option>
            <option value="Park Jimin" className="text-center" >Park Jimin</option>
            <option value="Kim Taehyung" className="text-center" >Kim Taeyhung</option>
            <option value="Jeon Jungkook" className="text-center" >Jeon JungKook</option>
            <option value="BTS" className="text-center" >BTS</option>
         
          </select>

          </div>
          <button
            type="submit"
            className="mt-5 rounded-xl w-full cursor-pointer p-2 bg-purple-400 text-white font-bold shadow-xl"
          >Enviar</button>
        </form>
  
        ) : (
          <div className="flex flex-col items-center justify-center mt-6">
              <Card
                name={enviar.name}
                country={enviar.country}
                cards={enviar.cards}
                image={enviar.image}
               />

              <div className="text-white text-4xl mt-6 flex flex-row items-center gap-72 cursor-pointer">
               <button 
                  onClick={() => setEnviar(null)}
               >
                 <IoMdArrowRoundBack/>
               </button>

               <button
                  onClick={() => downloadImage("download", 'image.png' )}
               >
                 <FcDownload />
               </button>
              </div>
            
          </div>
      
         )  } 

    </div>
  )
}
