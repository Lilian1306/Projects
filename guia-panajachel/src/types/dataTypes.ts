

export interface DataTown {
    id: number,
    name: string,
    top: string,
    left: string,
    description: string,
    image: string,
    heroImage: string
    fullDescription: string
    details: {
        geography: {
            department: string
            location: string
            climate?: string
        }
        demographics?: {
            population?: string
        }
        activities: string[]
    }
}
