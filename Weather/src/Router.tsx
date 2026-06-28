import { BrowserRouter, Route, Routes } from "react-router";
import MainPage from "./views/MainPage";



export default function router() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<MainPage/>}/>
      </Routes>
    </BrowserRouter>
  )
}


