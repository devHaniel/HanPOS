<script setup lang="ts">
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { categoryService, clientService, productService, providerService, userService } from '@/Service'
import type { CategoryRequest } from '@/Type/Category'
import type { Category } from '@/Type/Category'
import type { ClientRequest } from '@/Type/Client'
import type { ProductRequest } from '@/Type/Product'
import type { ProviderRequest } from '@/Type/Provider'
import type { UserCreateRequest, UserUpdateRequest } from '@/Type/User'

type ResourceKey = 'products' | 'categories' | 'clients' | 'providers' | 'users'
type Row = Record<string, unknown> & { id: number }
type Field = { key: string; label: string; type?: 'text' | 'number' | 'email' | 'password' | 'checkbox' | 'category'; required?: boolean }
type Adapter = { list: () => Promise<unknown>; create: (data: Record<string, unknown>) => Promise<unknown>; update: (id: number, data: Record<string, unknown>) => Promise<unknown>; remove: (id: number) => Promise<void> }
const props = defineProps<{ resource: ResourceKey }>()
const definitions: Record<ResourceKey, { title: string; description: string; columns: [string, string][]; fields: Field[] }> = {
  products: { title: 'Productos', description: 'Administra precios, existencias y catálogo.', columns: [['codigo','Código'],['nombre','Producto'],['categoriaId','Categoría'],['precioVenta','Precio'],['stock','Stock'],['activo','Estado']], fields: [{key:'codigo',label:'Código',required:true},{key:'nombre',label:'Nombre',required:true},{key:'precioVenta',label:'Precio de venta',type:'number',required:true},{key:'precioCompra',label:'Costo',type:'number',required:true},{key:'stock',label:'Existencia',type:'number',required:true},{key:'stockMinimo',label:'Mínimo',type:'number',required:true},{key:'categoriaId',label:'Categoría',type:'category'},{key:'activo',label:'Disponible',type:'checkbox'}] },
  categories: { title: 'Categorías', description: 'Organiza los productos del catálogo.', columns: [['nombre','Categoría'],['descripcion','Descripción'],['cantidadProductos','Productos'],['activo','Estado']], fields: [{key:'nombre',label:'Nombre',required:true},{key:'descripcion',label:'Descripción'},{key:'activo',label:'Activa',type:'checkbox'}] },
  clients: { title: 'Clientes', description: 'Consulta y mantiene los datos de tus clientes.', columns: [['nombre','Cliente'],['telefono','Teléfono'],['email','Correo'],['rtn','RTN']], fields: [{key:'nombre',label:'Nombre',required:true},{key:'telefono',label:'Teléfono'},{key:'email',label:'Correo',type:'email'},{key:'rtn',label:'RTN'}] },
  providers: { title: 'Proveedores', description: 'Gestiona los contactos de abastecimiento.', columns: [['nombre','Proveedor'],['rtn','RTN'],['telefono','Teléfono']], fields: [{key:'nombre',label:'Nombre',required:true},{key:'rtn',label:'RTN'},{key:'telefono',label:'Teléfono'}] },
  users: { title: 'Usuarios', description: 'Administra las cuentas activas del sistema.', columns: [['nombre','Nombre'],['userName','Usuario'],['email','Correo'],['activo','Estado']], fields: [{key:'nombre',label:'Nombre',required:true},{key:'username',label:'Usuario',required:true},{key:'email',label:'Correo',type:'email',required:true},{key:'password',label:'Contraseña inicial',type:'password',required:true},{key:'activo',label:'Activo',type:'checkbox'}] },
}
const adapters: Record<ResourceKey, Adapter> = {
  products: { list: () => productService.list({pagina:1,cantidad:100}), create: (data) => productService.create(data as unknown as ProductRequest), update: (id,data) => productService.update(id,data as unknown as ProductRequest), remove: productService.remove },
  categories: { list: () => categoryService.list(), create: (data) => categoryService.create(data as unknown as CategoryRequest), update: (id,data) => categoryService.update(id,data as unknown as CategoryRequest), remove: categoryService.remove },
  clients: { list: () => clientService.list(), create: (data) => clientService.create(data as unknown as ClientRequest), update: (id,data) => clientService.update(id,data as unknown as ClientRequest), remove: clientService.remove },
  providers: { list: () => providerService.list(), create: (data) => providerService.create(data as unknown as ProviderRequest), update: (id,data) => providerService.update(id,data as unknown as ProviderRequest), remove: providerService.remove },
  users: { list: () => userService.list(), create: (data) => userService.create(data as unknown as UserCreateRequest), update: (id,data) => userService.update(id,{...data,id} as unknown as UserUpdateRequest), remove: userService.remove },
}
const rows = ref<Row[]>([])
const loading = ref(false)
const saving = ref(false)
const error = ref('')
const notice = ref('')
const search = ref('')
const modalOpen = ref(false)
const editId = ref<number | null>(null)
const form = reactive<Record<string, string | number | boolean>>({})
const categories = ref<Category[]>([])
const selectedCategoryId = ref<number | null>(null)
const definition = computed(() => definitions[props.resource])
const visibleRows = computed(() => {
  const needle = search.value.toLowerCase().trim()
  if (!needle) return rows.value
  return rows.value.filter((row) => {
    const matchesText = Object.values(row).some((value) => String(value ?? '').toLowerCase().includes(needle))
    const matchesCategory = props.resource !== 'products' || selectedCategoryId.value === null || row.categoriaId === selectedCategoryId.value
    return matchesText && matchesCategory
  })
})
function display(row: Row, key: string) {
  const value = key === 'userName' ? row.userName ?? row.username : key === 'categoriaId' ? categories.value.find((category) => category.id === row.categoriaId)?.nombre : row[key]
  if (key === 'activo') return value ? 'Activo' : 'Inactivo'
  if (key === 'cantidadProductos') return Number(value ?? 0)
  if (key === 'precioVenta') return new Intl.NumberFormat('es-HN',{style:'currency',currency:'HNL'}).format(Number(value ?? 0))
  return value ?? '—'
}
async function load() {
  loading.value = true; error.value = ''
  try {
    const result = await adapters[props.resource].list() as Row[] | {items: Row[]}
    const records = Array.isArray(result) ? result : result.items
    if (props.resource === 'products') {
      categories.value = await categoryService.list()
      rows.value = records
    } else if (props.resource === 'categories') {
      const counts = await Promise.all(records.map(async (category) => {
        try { return (await categoryService.getProducts(category.id)).length } catch { return 0 }
      }))
      rows.value = records.map((category, index) => ({ ...category, cantidadProductos: counts[index] ?? 0 }))
    } else {
      rows.value = records
    }
  } catch (cause) { error.value = cause instanceof Error ? cause.message : 'No se pudo cargar la información.' }
  finally { loading.value = false }
}
function openCreate() {
  editId.value = null
  for (const key of Object.keys(form)) delete form[key]
  for (const field of definition.value.fields) form[field.key] = field.type === 'checkbox' ? true : field.type === 'number' ? 0 : field.type === 'category' ? '' : ''
  modalOpen.value = true
}
function openEdit(row: Row) {
  editId.value = row.id
  for (const key of Object.keys(form)) delete form[key]
  for (const field of definition.value.fields) form[field.key] = (row[field.key] ?? (field.key === 'username' ? row.userName : undefined) ?? (field.type === 'checkbox' ? true : field.type === 'number' ? 0 : '')) as string | number | boolean
  modalOpen.value = true
}
async function save() {
  saving.value = true; error.value = ''
  const payload: Record<string, unknown> = {}
  for (const field of definition.value.fields) payload[field.key] = field.type === 'number' ? Number(form[field.key]) : field.key === 'categoriaId' && !form[field.key] ? null : form[field.key]
  if (props.resource === 'users' && editId.value && !payload.password) delete payload.password
  try {
    if (editId.value) await adapters[props.resource].update(editId.value, payload)
    else await adapters[props.resource].create(payload)
    modalOpen.value = false; notice.value = editId.value ? 'Cambios guardados.' : 'Registro creado.'; await load()
  } catch (cause) { error.value = cause instanceof Error ? cause.message : 'No se pudo guardar el registro.' }
  finally { saving.value = false }
}
async function remove(row: Row) {
  if (!window.confirm(`¿Eliminar ${String(row.nombre ?? row.codigo ?? row.id)}?`)) return
  error.value = ''
  try { await adapters[props.resource].remove(row.id); notice.value = 'Registro eliminado.'; await load() }
  catch (cause) { error.value = cause instanceof Error ? cause.message : 'No se pudo eliminar el registro.' }
}
watch(() => props.resource, load)
onMounted(load)
</script>
<template>
  <section class="fade-up"><div class="page-heading"><div><p class="eyebrow">Administración</p><h1>{{ definition.title }}</h1><p>{{ definition.description }}</p></div><button class="btn" @click="openCreate">＋ Nuevo registro</button></div>
    <div v-if="error" class="alert-box" style="margin-bottom:12px">{{ error }}</div><div v-if="notice" class="status-pill" style="margin-bottom:12px">{{ notice }}</div>
    <section class="panel"><div class="panel-header"><div><h2>{{ definition.title }} registrados</h2><p>{{ visibleRows.length }} registros</p></div><div class="toolbar-group"><select v-if="resource === 'products'" v-model="selectedCategoryId" class="field product-category-filter"><option :value="null">Todas las categorías</option><option v-for="category in categories" :key="category.id" :value="category.id">{{ category.nombre }}</option></select><input v-model="search" class="search-field" placeholder="Buscar en la lista"/><button class="btn btn-secondary" @click="load">Actualizar</button></div></div>
      <div v-if="loading" class="loading-state">Cargando registros…</div><div v-else class="table-wrap"><table class="data-table"><thead><tr><th v-for="column in definition.columns" :key="column[0]">{{ column[1] }}</th><th>Acciones</th></tr></thead><tbody>
        <tr v-for="row in visibleRows" :key="row.id"><td v-for="column in definition.columns" :key="column[0]" :class="{ 'row-primary': column[0] === 'nombre' || column[0] === 'codigo' }"><span v-if="column[0] === 'activo'" class="status-pill" :class="{inactive: !row.activo}">{{ display(row,column[0]) }}</span><template v-else>{{ display(row,column[0]) }}</template></td><td><div class="table-actions"><button class="table-action" @click="openEdit(row)">Editar</button><button class="table-action delete" @click="remove(row)">Eliminar</button></div></td></tr>
      </tbody></table><div v-if="!visibleRows.length" class="empty-state">No hay registros para mostrar.</div></div>
    </section>
    <div v-if="modalOpen" class="modal-backdrop" @click.self="modalOpen = false"><section class="modal" role="dialog" aria-modal="true" :aria-label="editId ? 'Editar registro' : 'Nuevo registro'"><div class="panel-header"><div><h2>{{ editId ? 'Editar' : 'Nuevo' }} {{ definition.title.toLowerCase() }}</h2><p>Completa los campos del registro.</p></div><button class="icon-button" aria-label="Cerrar" @click="modalOpen = false">×</button></div>
      <form @submit.prevent="save"><div class="modal-form"><label v-for="field in definition.fields" :key="field.key" v-show="!(field.key === 'password' && editId)" class="form-field" :class="{full: field.type === 'checkbox'}">{{ field.label }}<select v-if="field.type === 'category'" v-model="form[field.key]" class="field"><option value="">Sin categoría</option><option v-for="category in categories" :key="category.id" :value="category.id">{{ category.nombre }}</option></select><input v-else-if="field.type !== 'checkbox'" v-model="form[field.key]" class="field" :type="field.type || 'text'" :required="field.required && !(field.key === 'password' && editId)" :min="field.type === 'number' ? 0 : undefined" :step="field.type === 'number' ? 'any' : undefined"/><span v-else class="checkbox-field"><input v-model="form[field.key]" type="checkbox"/> Sí</span></label></div>
        <div v-if="error" class="alert-box" style="margin:0 20px">{{ error }}</div><div class="modal-actions"><button type="button" class="btn btn-secondary" @click="modalOpen = false">Cancelar</button><button class="btn" type="submit" :disabled="saving">{{ saving ? 'Guardando…' : 'Guardar' }}</button></div></form>
    </section></div>
  </section>
</template>
