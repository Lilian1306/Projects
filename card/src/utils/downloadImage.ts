import html2canvas from "html2canvas"


export const downloadImage = async (elementId: string, fieldName: string) => {
    const element = document.getElementById(elementId)

    if(!element) {
        console.error(`No se encontro el elemnto: ${elementId}`)
        return
    }
    try{
        const canvas = await html2canvas(element)
        const data = canvas.toDataURL('image/png')
        const link = document.createElement('a')

        if(typeof link.download === 'string') {
            link.href = data
            link.download = fieldName
            document.body.appendChild(link)
            link.click()
            document.body.removeChild(link)
        } else {
            window.open(data)
        }
    } catch(error){
        console.log('Error al generar la imagen:', error)
    }
}