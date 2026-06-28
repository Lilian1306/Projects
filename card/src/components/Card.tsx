

export type CardProps = {
    name: string, 
    country: string, 
    cards:  string,
    image: string
}

export default function Card({name, country, cards, image} : CardProps) {
  return (
    <> 
    <div 
      id="download"
      className="flex flex-col items-center justify-between bg-[#7729cb] w-[400px] h-[600px] mx-auto shadow-xl p-10"
    >
     <div className=" w-full text-left font-bold text-2xl text-[#ffffff] italic">{name}</div>
     <div className="flex-1 w-full flex items-center justify-center overflow-hidden my-4">
        <img
        src={image}
        alt={cards}
        className="max-w-full max-h-full object-contain rounded-lg shadow-md"
      />

     </div>
      <p className="text-[#ffffff] font-bold text-2xl text-right w-full mt-auto italic">{country}</p>
    </div>
    </>
  )
}

// Comentario para poder subir cambios