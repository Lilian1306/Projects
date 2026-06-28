import { lazy, Suspense } from 'react'
import { BrowserRouter, Routes, Route } from "react-router-dom"
const IndexPage = lazy(() => import ("./pages/IndexPage"))
const FavoritesPage = lazy(() => import("./pages/FavoritePage") )

import Layout from "./layouts/Layout"

export default function AppRouter() {
  return (
    <BrowserRouter>
      <Routes >
        <Route element={<Layout/>}>
         <Route path="/" element={
           <Suspense fallback='Cargando...'>
            <IndexPage/>
           </Suspense>
         } />
         <Route path="/favoritos" element={
           <Suspense fallback='Cargando...'>
             <FavoritesPage/>
           </Suspense>
         }/>
        </Route>
      </Routes>
    </BrowserRouter>
  )
}