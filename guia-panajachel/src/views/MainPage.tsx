import { useState } from "react";
import dataButtons from "../data/DataButton";
import type { DataTown } from "../types/dataTypes";
import TownModal from "../components/TownModal";


export default function MainPage() {

  const [selectedTown, setSelectedTown] = useState<DataTown | null>(null)
  const [isOpen, setIsOpen] = useState(false)

  function openModal(town: DataTown) {
    setSelectedTown(town)
    setIsOpen(true)
  }

  function closeModal() {
    setIsOpen(false)
  }

  return (
    <div className="relative w-full mx-auto mt-6">
      <img
        src="/Panajachel.png"
        alt="Mapa del lago de Atitlan"
        className="w-full h-auto object-cover rounded-4xl"
      />

      {dataButtons.map((town) => (
        <button
          key={town.id}
          style={{top: town.top, left: town.left}}
          className="group absolute w-[12%] h-[6%] hover:bg-green-900/90 cursor-pointer rounded-xl z-10"
          onClick={() => openModal(town)}
        >
          <span
            className="absolute -top-5 left-1/2 -translate-x-1/2 bg-black/90 text-white text-sm font-bold px-2 py-1 rounded opacity-0 group-hover:opacity-100 transition-opacity duration-200 pointer-events-none whitespace-nowrap"
          >Ver Detalles</span>
        </button>
    
      ))}
     <TownModal
        town={selectedTown}
        closeModal={closeModal}
        isOpen={isOpen}
     />
    </div>
  )
}

//Leaving just a comment