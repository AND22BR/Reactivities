type Activity= {
  id: string
  title: string
  date: date
  description: string
  category: string
  city: string
  venue: string
  latitude: number
  longitude: number
}

type User={
  id: string
  email: string
  displayName: string
  imageUrl?: string
}

type LocationIQSuggestion ={
  place_id: string
  osm_id: string
  osm_type: string
  licence: string
  lat: string
  lon: string
  boundingbox: string[]
  class: string
  type: string
  display_name: string
  display_place: string
  display_address: string
  address: LocationIQAddress
}

type LocationIQAddress ={
  name: string
  road?: string
  neighbourhood?: string
  suburb?: string
  town?: string
  village?: string
  city?: string
  state: string
  postcode: string
  country: string
  country_code: string
  county?: string
}


