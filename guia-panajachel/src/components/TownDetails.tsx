import { useParams } from "react-router-dom"
import dataButtons from "../data/DataButton"
import BackButton from "./BackButton"

export default function TownDetails() {
  const { id } = useParams()
  const townData = dataButtons.find(info => info.id === Number(id))
  
  if(!townData) 
    return <div className="text-center py-10 dark:text-white">Pueblo No Encontrado</div>

  return (
    <div className="min-h-screen bg-white dark:bg-slate-950 transition-colors duration-500">
      
      <div className="max-w-4xl mx-auto px-6 py-8">
        
        <h1 className="text-3xl md:text-5xl font-bold mb-6 text-center text-slate-900 dark:text-white">
          {townData.name}
        </h1>
        <div className="relative h-64 md:h-96 w-full overflow-hidden rounded-2xl shadow-lg border dark:border-slate-800">
          <img
            src={townData.heroImage}
            alt={townData.name}
            className="w-full h-full object-cover"
          />
        </div>
        <div className="text-slate-800 dark:text-slate-300 text-lg mt-6 leading-relaxed">
          <p>{townData.fullDescription}</p>
        </div>
        <div className="border-t border-gray-200 dark:border-slate-800 pt-8 mt-8">
          <h2 className="text-2xl font-bold mb-4 text-slate-900 dark:text-white">Actividades</h2>
          <ul className="space-y-4">
            {townData.details.activities.map((activity, index) => (
              <li key={index} className="flex items-start bg-slate-50 dark:bg-slate-900/50 p-4 rounded-xl">
                <span className="h-3 w-3 rounded-full bg-sky-500 mt-2 mr-4 flex-shrink-0"></span>
                <span className="text-lg  text-slate-900 dark:text-white">{activity}</span>
              </li>
            ))}
          </ul>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6 py-8 border-t border-b border-gray-100 dark:border-slate-800 my-8">
            <div className="space-y-1">
              <h3 className="text-sm font-bold uppercase tracking-wider text-slate-500 dark:text-slate-400">Ubicación</h3>
              <p className="text-sm text-slate-900 dark:text-white">{townData.details.geography.location}</p>
              <p className="text-sm text-slate-600">{townData.details.geography.department}</p>
            </div>

            {townData.details.geography.climate && (
              <div className="space-y-1">
                <h3 className="text-sm uppercase font-bold tracking-wider text-slate-500 dark:text-slate-400">Clima</h3>
                <p className="text-sm text-slate-900 dark:text-white">{townData.details.geography.climate}</p>
              </div>
            )}

            {townData.details.demographics?.population && (
              <div className="space-y-1">
                <h3 className="text-sm font-bold uppercase tracking-wider text-slate-500 dark:text-slate-400">Población</h3>
                <p className="text-sm  text-slate-900 dark:text-white">{townData.details.demographics.population}</p>
              </div>
            )}
          </div>
        </div>  

        <div className="pb-10">
          <BackButton/>
        </div>
      </div>
    </div>
  )
}