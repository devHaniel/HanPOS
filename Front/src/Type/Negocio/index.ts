export interface Negocio {
  id: number
  nombre: string
  razonSocial: string | null
  identificacionFiscal: string | null
  direccion: string | null
  telefono: string | null
  email: string | null
  mensajePieComprobante: string | null
  logo: string | null
  preferenciasPantalla: string | null
  configuracionImpresion: string | null
  monedaDefecto: string
  simboloMoneda: string
  zonaHoraria: string
  activo: boolean
  fechaCreacion: string
  fechaActualizacion: string
}

export interface NegocioRequest {
  nombre: string
  razonSocial?: string | null
  identificacionFiscal?: string | null
  direccion?: string | null
  telefono?: string | null
  email?: string | null
  mensajePieComprobante?: string | null
  logo?: string | null
  preferenciasPantalla?: string | null
  configuracionImpresion?: string | null
  monedaDefecto: string
  simboloMoneda: string
  zonaHoraria: string
  activo: boolean
}