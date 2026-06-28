import { useNavigate } from "react-router-dom"
import { TiArrowBack } from "react-icons/ti";

export default function BackButton() {
    const navigate = useNavigate()

  return (
    <button
        onClick={() => navigate(-1)}
        className="text-3xl"
    >
        <TiArrowBack className="text-blue-700"/>
        <span className="text-blue-700">Regresar</span>
    </button>
  )
}
 