(() => {
  'use strict';
  const $ = (s, root = document) => root.querySelector(s);
  const $$ = (s, root = document) => [...root.querySelectorAll(s)];
  const state = {
    token: sessionStorage.getItem('wm_token') || '', user: null,
    page: 'dashboard', productPage: 1, productPageSize: 10,
    movementPage: 1, movementPageSize: 10,
    categories: [], suppliers: [], warehouses: [], products: [],
    stocks: [], productSearchTimer: null
  };
  const money = n => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND', maximumFractionDigits: 0 }).format(Number(n || 0));
  const number = n => new Intl.NumberFormat('vi-VN', { maximumFractionDigits: 3 }).format(Number(n || 0));
  const date = v => v ? new Date(v).toLocaleString('vi-VN') : '—';
  const esc = v => String(v ?? '').replace(/[&<>"']/g, c => ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
  const roleIsAdmin = () => state.user?.role === 'Admin';
  function toast(message, kind = 'success') {
    const el = document.createElement('div'); el.className = `toast ${kind}`; el.textContent = message;
    $('#toastHost').append(el); setTimeout(() => el.remove(), 3800);
  }
  function errorMessage(data, fallback) {
    if (data?.message) return data.message;
    if (data?.title) return data.title;
    if (data?.errors) return Object.values(data.errors).flat().join(' ');
    return fallback || 'Đã xảy ra lỗi. Vui lòng thử lại.';
  }
  async function api(path, options = {}) {
    const headers = new Headers(options.headers || {});
    headers.set('Accept', 'application/json');
    if (state.token) headers.set('Authorization', `Bearer ${state.token}`);
    if (options.body && !(options.body instanceof FormData)) headers.set('Content-Type', 'application/json');
    let response;
    try { response = await fetch(path, { ...options, headers }); }
    catch { throw new Error('Không kết nối được API. Hãy kiểm tra ứng dụng ASP.NET Core còn đang chạy hay không.'); }
    const text = await response.text();
    let data = null;
    if (text) { try { data = JSON.parse(text); } catch { data = text; } }
    if (response.status === 401 && state.token) {
      state.token = ''; state.user = null; sessionStorage.removeItem('wm_token'); showLogin();
    }
    if (!response.ok) throw new Error(errorMessage(data, `API trả về HTTP ${response.status}.`));
    return data;
  }
  const get = path => api(path);
  const post = (path, body) => api(path, { method: 'POST', body: JSON.stringify(body) });
  const put = (path, body) => api(path, { method: 'PUT', body: JSON.stringify(body) });
  const del = path => api(path, { method: 'DELETE' });

  function showLogin() {
    $('#loginScreen').classList.remove('hidden'); $('#appShell').classList.add('hidden');
    $('#loginError').textContent = '';
  }
  function showApp() {
    $('#loginScreen').classList.add('hidden'); $('#appShell').classList.remove('hidden');
    const label = state.user?.username || 'Người dùng';
    $('#userName').textContent = label; $('#userRole').textContent = state.user?.role || '';
    $('#userAvatar').textContent = label.slice(0,1).toUpperCase(); $('#topAvatar').textContent = label.slice(0,1).toUpperCase();
    $$('.admin-only').forEach(x => x.classList.toggle('hidden', !roleIsAdmin()));
    $('#todayLabel').textContent = new Date().toLocaleDateString('vi-VN', { weekday:'short', day:'2-digit', month:'short', year:'numeric' });
    loadReferenceData().then(() => navigate('dashboard')).catch(e => toast(e.message, 'error'));
  }
  async function loadReferenceData() {
    const [categories, suppliers, warehouses] = await Promise.all([get('/api/categories'), get('/api/suppliers'), get('/api/warehouses')]);
    state.categories = categories || []; state.suppliers = suppliers || []; state.warehouses = warehouses || [];
    fillSelects();
  }
  function fillSelects() {
    const cat = $('#productCategory');
    if (cat) cat.innerHTML = '<option value="">Tất cả danh mục</option>' + state.categories.map(x => `<option value="${x.id}">${esc(x.name)}</option>`).join('');
    const stockWh = $('#stockWarehouse');
    if (stockWh) stockWh.innerHTML = '<option value="">Tất cả kho</option>' + state.warehouses.map(x => `<option value="${x.id}">${esc(x.name)}</option>`).join('');
  }
  function navigate(page, options = {}) {
    state.page = page;
    $$('.page-section').forEach(x => x.classList.toggle('hidden', x.id !== `page-${page}`));
    $$('.nav-link').forEach(x => x.classList.toggle('active', x.dataset.page === page));
    const titles = {dashboard:'Tổng quan',products:'Sản phẩm',categories:'Danh mục',suppliers:'Nhà cung cấp',warehouses:'Kho hàng',inventory:'Tồn kho & nhập xuất',movements:'Lịch sử giao dịch'};
    $('#breadcrumbTitle').textContent = titles[page] || 'Tổng quan'; $('#sidebar').classList.remove('open');
    const loaders = {dashboard:loadDashboard,products:loadProducts,categories:loadCategories,suppliers:loadSuppliers,warehouses:loadWarehouses,inventory:loadInventory,movements:loadMovements};
    (loaders[page] || loadDashboard)().catch(e => toast(e.message, 'error'));
    if (options.action && page === 'inventory') setTimeout(() => openMovementModal(options.action), 120);
  }
  function tableEmpty(message = 'Chưa có dữ liệu') { return `<div class="empty-state">${esc(message)}</div>`; }
  function pager(host, page, pageSize, total, onChange) {
    const pages = Math.max(1, Math.ceil(total / pageSize));
    $(host).innerHTML = `<span>Tổng ${number(total)} dòng · Trang ${page}/${pages}</span><button data-dir="prev" ${page<=1?'disabled':''}>← Trước</button><button data-dir="next" ${page>=pages?'disabled':''}>Sau →</button>`;
    $$('button[data-dir]', $(host)).forEach(b => b.addEventListener('click', () => onChange(page + (b.dataset.dir === 'next' ? 1 : -1))));
  }
  async function loadDashboard() {
    const [s, stocks] = await Promise.all([get('/api/dashboard/summary'), get('/api/inventory')]);
    const cards = [
      ['Sản phẩm đang hoạt động',s.totalProducts,'▤','#eaf3fb','#3c79a6'],
      ['Danh mục sản phẩm',s.totalCategories,'▧','#f2edfc','#8170b5'],
      ['Tổng số kho',s.totalWarehouses,'⌂','#e8f6ee','#21825d'],
      ['Tổng tồn kho',number(s.totalStockQuantity),'▣','#fff4df','#a87620'],
      ['Nhà cung cấp',s.totalSuppliers,'♧','#eaf3fb','#3c79a6'],
      ['Người dùng hoạt động',s.totalUsers,'♙','#f2edfc','#8170b5'],
      ['Dòng tồn thấp',s.lowStockLines,'⚑','#fff0ef','#be5c56'],
      ['Giao dịch nhập/xuất',s.totalMovements,'↔','#e8f6ee','#21825d']
    ];
    $('#summaryCards').innerHTML = cards.map(c => `<div class="stat-card" style="--tint:${c[3]};--tone:${c[4]}"><span class="stat-icon">${c[2]}</span><div class="stat-label">${c[0]}</div><div class="stat-value">${number(c[1])}</div><div class="stat-foot">Dữ liệu cập nhật từ hệ thống</div></div>`).join('');
    const low = (stocks || []).filter(x => x.isLowStock);
    $('#lowStockTable').innerHTML = low.length ? `<table><thead><tr><th>SẢN PHẨM</th><th>KHO</th><th>TỒN HIỆN TẠI</th><th>MỨC TỐI THIỂU</th><th>TRẠNG THÁI</th></tr></thead><tbody>${low.slice(0,8).map(x=>`<tr><td><div class="table-primary">${esc(x.productName)}</div><div class="table-secondary">${esc(x.sku)}</div></td><td>${esc(x.warehouseName)}</td><td>${number(x.quantity)} ${esc(x.unit)}</td><td>${number(x.minStockLevel)}</td><td><span class="pill pill-red">Sắp hết hàng</span></td></tr>`).join('')}</tbody></table>` : tableEmpty('Không có dòng tồn kho nào dưới mức tối thiểu.');
  }
  async function loadProducts() {
    const params = new URLSearchParams({pageNumber:String(state.productPage),pageSize:String(state.productPageSize),sortBy:$('#productSort').value,isDescending:$('#productDirection').value});
    const search = $('#productSearch').value.trim(), category = $('#productCategory').value;
    if (search) params.set('searchTerm', search); if (category) params.set('categoryId', category);
    const result = await get(`/api/products?${params}`); state.products = result.items || [];
    $('#productsTable').innerHTML = state.products.length ? `<table><thead><tr><th>SẢN PHẨM</th><th>DANH MỤC</th><th>NHÀ CUNG CẤP</th><th>GIÁ BÁN</th><th>TỒN TỐI THIỂU</th><th>TRẠNG THÁI</th><th>THAO TÁC</th></tr></thead><tbody>${state.products.map(p=>`<tr><td><div class="table-primary">${esc(p.name)}</div><div class="table-secondary">${esc(p.sku)} · ${esc(p.unit)}</div></td><td>${esc(p.categoryName)}</td><td>${esc(p.supplierName || '—')}</td><td>${money(p.sellingPrice)}</td><td>${number(p.minStockLevel)}</td><td><span class="pill ${p.isActive?'pill-green':'pill-gray'}">${p.isActive?'Đang kinh doanh':'Ngừng kinh doanh'}</span></td><td>${roleIsAdmin()?`<div class="action-group"><button class="table-action" data-edit-product="${p.id}">Sửa</button><button class="table-action danger" data-delete-product="${p.id}">Xóa</button></div>`:'—'}</td></tr>`).join('')}</tbody></table>` : tableEmpty('Không tìm thấy sản phẩm phù hợp.');
    pager('#productsPager',result.pageNumber,result.pageSize,result.totalCount,p=>{state.productPage=p;loadProducts().catch(e=>toast(e.message,'error'));});
    $$('[data-edit-product]').forEach(b=>b.addEventListener('click',()=>openProductModal(state.products.find(x=>x.id===Number(b.dataset.editProduct)))));
    $$('[data-delete-product]').forEach(b=>b.addEventListener('click',()=>deleteProduct(Number(b.dataset.deleteProduct))));
  }
  async function deleteProduct(id) { if (!confirm('Bạn chắc chắn muốn xóa sản phẩm này?')) return; try { await del(`/api/products/${id}`); toast('Đã xóa sản phẩm.'); await loadProducts(); } catch(e) { toast(e.message,'error'); } }
  function openProductModal(p = null) {
    const editing = !!p; const catOptions = state.categories.map(x=>`<option value="${x.id}" ${p?.categoryId===x.id?'selected':''}>${esc(x.name)}</option>`).join('');
    const supOptions = state.suppliers.map(x=>`<option value="${x.id}" ${p?.supplierId===x.id?'selected':''}>${esc(x.name)}</option>`).join('');
    openModal(editing?'Cập nhật sản phẩm':'Thêm sản phẩm', `<form id="productForm" class="modal-form">
      <label>Mã SKU<input name="sku" required maxlength="50" value="${esc(p?.sku||'')}" placeholder="SP-003"></label>
      <label>Tên sản phẩm<input name="name" required maxlength="200" value="${esc(p?.name||'')}"></label>
      <label>Đơn vị tính<input name="unit" required value="${esc(p?.unit||'cái')}"></label>
      <label>Danh mục<select name="categoryId" required>${catOptions}</select></label>
      <label>Nhà cung cấp<select name="supplierId"><option value="">Không chọn</option>${supOptions}</select></label>
      <label>Giá nhập<input name="costPrice" type="number" min="0" step="0.01" required value="${p?.costPrice??0}"></label>
      <label>Giá bán<input name="sellingPrice" type="number" min="0" step="0.01" required value="${p?.sellingPrice??0}"></label>
      <label>Tồn tối thiểu<input name="minStockLevel" type="number" min="0" step="0.001" required value="${p?.minStockLevel??0}"></label>
      <label class="full checkbox-label"><input name="isActive" type="checkbox" ${p?.isActive!==false?'checked':''}> Đang kinh doanh</label>
      <div class="modal-actions full"><button type="button" class="btn btn-outline" data-close-modal>Hủy</button><button class="btn btn-primary" type="submit">${editing?'Lưu thay đổi':'Tạo sản phẩm'}</button></div>
    </form>`);
    $('#productForm').addEventListener('submit',async e=>{e.preventDefault();const f=new FormData(e.currentTarget);const body={sku:f.get('sku'),name:f.get('name'),unit:f.get('unit'),costPrice:Number(f.get('costPrice')),sellingPrice:Number(f.get('sellingPrice')),minStockLevel:Number(f.get('minStockLevel')),categoryId:Number(f.get('categoryId')),supplierId:f.get('supplierId')?Number(f.get('supplierId')):null,isActive:f.get('isActive')==='on'};try{if(editing)await put(`/api/products/${p.id}`,body);else await post('/api/products',body);closeModal();toast(editing?'Đã cập nhật sản phẩm.':'Đã thêm sản phẩm.');await loadProducts();}catch(err){toast(err.message,'error');}});
  }
  async function loadCategories() {
    state.categories=await get('/api/categories');
    $('#categoriesTable').innerHTML=state.categories.length?`<table><thead><tr><th>#</th><th>TÊN DANH MỤC</th><th>MÔ TẢ</th><th>NGÀY TẠO</th><th>THAO TÁC</th></tr></thead><tbody>${state.categories.map(x=>`<tr><td>${x.id}</td><td class="table-primary">${esc(x.name)}</td><td>${esc(x.description||'—')}</td><td>${date(x.createdAt)}</td><td>${roleIsAdmin()?`<div class="action-group"><button class="table-action" data-edit-category="${x.id}">Sửa</button><button class="table-action danger" data-delete-category="${x.id}">Xóa</button></div>`:'—'}</td></tr>`).join('')}</tbody></table>`:tableEmpty();
    $$('[data-edit-category]').forEach(b=>b.addEventListener('click',()=>openCategoryModal(state.categories.find(x=>x.id===Number(b.dataset.editCategory)))));
    $$('[data-delete-category]').forEach(b=>b.addEventListener('click',()=>deleteCatalog('categories',b.dataset.deleteCategory,loadCategories,'Bạn chắc chắn muốn xóa danh mục này?')));
    fillSelects();
  }
  function openCategoryModal(x=null){openModal(x?'Sửa danh mục':'Thêm danh mục',`<form id="categoryForm" class="modal-form"><label class="full">Tên danh mục<input name="name" required maxlength="100" value="${esc(x?.name||'')}"></label><label class="full">Mô tả<textarea name="description" maxlength="500">${esc(x?.description||'')}</textarea></label><div class="modal-actions full"><button type="button" class="btn btn-outline" data-close-modal>Hủy</button><button class="btn btn-primary">Lưu</button></div></form>`);$('#categoryForm').addEventListener('submit',async e=>{e.preventDefault();const f=new FormData(e.currentTarget),body={name:f.get('name'),description:f.get('description')||null};try{if(x)await put(`/api/categories/${x.id}`,body);else await post('/api/categories',body);closeModal();toast('Đã lưu danh mục.');await loadCategories();}catch(err){toast(err.message,'error');}});}
  async function loadSuppliers(){state.suppliers=await get('/api/suppliers');$('#suppliersTable').innerHTML=state.suppliers.length?`<table><thead><tr><th>#</th><th>NHÀ CUNG CẤP</th><th>NGƯỜI LIÊN HỆ</th><th>ĐIỆN THOẠI</th><th>EMAIL</th><th>ĐỊA CHỈ</th><th>THAO TÁC</th></tr></thead><tbody>${state.suppliers.map(x=>`<tr><td>${x.id}</td><td class="table-primary">${esc(x.name)}</td><td>${esc(x.contactName||'—')}</td><td>${esc(x.phone||'—')}</td><td>${esc(x.email||'—')}</td><td>${esc(x.address||'—')}</td><td>${roleIsAdmin()?`<div class="action-group"><button class="table-action" data-edit-supplier="${x.id}">Sửa</button><button class="table-action danger" data-delete-supplier="${x.id}">Xóa</button></div>`:'—'}</td></tr>`).join('')}</tbody></table>`:tableEmpty();$$('[data-edit-supplier]').forEach(b=>b.addEventListener('click',()=>openSupplierModal(state.suppliers.find(x=>x.id===Number(b.dataset.editSupplier)))));$$('[data-delete-supplier]').forEach(b=>b.addEventListener('click',()=>deleteCatalog('suppliers',b.dataset.deleteSupplier,loadSuppliers,'Bạn chắc chắn muốn xóa nhà cung cấp này?')));fillSelects();}
  function openSupplierModal(x=null){openModal(x?'Sửa nhà cung cấp':'Thêm nhà cung cấp',`<form id="supplierForm" class="modal-form"><label class="full">Tên nhà cung cấp<input name="name" required maxlength="150" value="${esc(x?.name||'')}"></label><label>Người liên hệ<input name="contactName" value="${esc(x?.contactName||'')}"></label><label>Số điện thoại<input name="phone" value="${esc(x?.phone||'')}"></label><label>Email<input name="email" type="email" value="${esc(x?.email||'')}"></label><label>Địa chỉ<input name="address" value="${esc(x?.address||'')}"></label><div class="modal-actions full"><button type="button" class="btn btn-outline" data-close-modal>Hủy</button><button class="btn btn-primary">Lưu</button></div></form>`);$('#supplierForm').addEventListener('submit',async e=>{e.preventDefault();const f=new FormData(e.currentTarget),body={name:f.get('name'),contactName:f.get('contactName')||null,phone:f.get('phone')||null,email:f.get('email')||null,address:f.get('address')||null};try{if(x)await put(`/api/suppliers/${x.id}`,body);else await post('/api/suppliers',body);closeModal();toast('Đã lưu nhà cung cấp.');await loadSuppliers();}catch(err){toast(err.message,'error');}});}
  async function loadWarehouses(){state.warehouses=await get('/api/warehouses');$('#warehousesTable').innerHTML=state.warehouses.length?`<table><thead><tr><th>#</th><th>MÃ KHO</th><th>TÊN KHO</th><th>ĐỊA CHỈ</th><th>THAO TÁC</th></tr></thead><tbody>${state.warehouses.map(x=>`<tr><td>${x.id}</td><td><span class="pill pill-blue">${esc(x.code)}</span></td><td class="table-primary">${esc(x.name)}</td><td>${esc(x.address||'—')}</td><td>${roleIsAdmin()?`<div class="action-group"><button class="table-action" data-edit-warehouse="${x.id}">Sửa</button><button class="table-action danger" data-delete-warehouse="${x.id}">Xóa</button></div>`:'—'}</td></tr>`).join('')}</tbody></table>`:tableEmpty();$$('[data-edit-warehouse]').forEach(b=>b.addEventListener('click',()=>openWarehouseModal(state.warehouses.find(x=>x.id===Number(b.dataset.editWarehouse)))));$$('[data-delete-warehouse]').forEach(b=>b.addEventListener('click',()=>deleteCatalog('warehouses',b.dataset.deleteWarehouse,loadWarehouses,'Bạn chắc chắn muốn xóa kho này?')));fillSelects();}
  function openWarehouseModal(x=null){openModal(x?'Sửa kho hàng':'Thêm kho hàng',`<form id="warehouseForm" class="modal-form"><label>Mã kho<input name="code" required maxlength="30" value="${esc(x?.code||'')}"></label><label>Tên kho<input name="name" required maxlength="150" value="${esc(x?.name||'')}"></label><label class="full">Địa chỉ<input name="address" maxlength="300" value="${esc(x?.address||'')}"></label><div class="modal-actions full"><button type="button" class="btn btn-outline" data-close-modal>Hủy</button><button class="btn btn-primary">Lưu</button></div></form>`);$('#warehouseForm').addEventListener('submit',async e=>{e.preventDefault();const f=new FormData(e.currentTarget),body={code:f.get('code'),name:f.get('name'),address:f.get('address')||null};try{if(x)await put(`/api/warehouses/${x.id}`,body);else await post('/api/warehouses',body);closeModal();toast('Đã lưu kho hàng.');await loadWarehouses();}catch(err){toast(err.message,'error');}});}
  async function deleteCatalog(resource,id,reload,message){if(!confirm(message))return;try{await del(`/api/${resource}/${id}`);toast('Đã xóa dữ liệu.');await reload();}catch(e){toast(e.message,'error');}}
  async function loadInventory(){const wid=$('#stockWarehouse').value;const stocks=await get('/api/inventory'+(wid?`?warehouseId=${wid}`:''));state.stocks=stocks||[];$('#stocksTable').innerHTML=state.stocks.length?`<table><thead><tr><th>SẢN PHẨM</th><th>KHO</th><th>SỐ LƯỢNG</th><th>TỒN TỐI THIỂU</th><th>TRẠNG THÁI</th><th>CẬP NHẬT</th></tr></thead><tbody>${state.stocks.map(x=>`<tr><td><div class="table-primary">${esc(x.productName)}</div><div class="table-secondary">${esc(x.sku)}</div></td><td>${esc(x.warehouseName)}</td><td><b>${number(x.quantity)}</b> ${esc(x.unit)}</td><td>${number(x.minStockLevel)}</td><td><span class="pill ${x.isLowStock?'pill-red':'pill-green'}">${x.isLowStock?'Sắp hết hàng':'Đủ hàng'}</span></td><td>${date(x.updatedAt)}</td></tr>`).join('')}</tbody></table>`:tableEmpty('Chưa có dữ liệu tồn kho.');}
  async function getAllActiveProductsForMovement(){
    const first=await get('/api/products?pageNumber=1&pageSize=100&sortBy=sku&isDescending=false');
    const items=[...(first.items||[])];
    const totalPages=Math.max(1,Number(first.totalPages||1));
    for(let page=2;page<=totalPages;page++){
      const result=await get(`/api/products?pageNumber=${page}&pageSize=100&sortBy=sku&isDescending=false`);
      items.push(...(result.items||[]));
    }
    return items.filter(p=>p.isActive).sort((a,b)=>String(a.sku).localeCompare(String(b.sku),'vi'));
  }

  async function openMovementModal(kind){
    const isReceipt=kind==='receipt';
    if(!state.warehouses.length) state.warehouses=await get('/api/warehouses');
    const allProducts=await getAllActiveProductsForMovement();
    const warehouseOptions=state.warehouses
      .slice().sort((a,b)=>a.id-b.id)
      .map(w=>`<option value="${w.id}">${esc(w.code)} · ${esc(w.name)}</option>`).join('');

    openModal(isReceipt?'Tạo phiếu nhập kho':'Tạo phiếu xuất kho',`
      <form id="movementForm" class="modal-form">
        <label class="full">1. Chọn kho hàng
          <select name="warehouseId" id="movementWarehouse" required>
            <option value="">-- Chọn kho trước --</option>${warehouseOptions}
          </select>
        </label>
        <label class="full">2. Chọn sản phẩm
          <select name="productId" id="movementProduct" required disabled>
            <option value="">Vui lòng chọn kho trước</option>
          </select>
        </label>
        <p id="movementProductHint" class="muted full" style="margin:-8px 0 4px;font-size:12px">${isReceipt?'Chọn kho để xem sản phẩm đã có và chưa có tại kho.':'Chọn kho để chỉ xem sản phẩm đang còn tồn tại kho đó.'}</p>
        <label>Số lượng<input name="quantity" id="movementQuantity" type="number" min="1" step="1" required value="1"></label>
        <label>Số phiếu / tham chiếu<input name="referenceNo" maxlength="50" placeholder="${isReceipt?'PN-0001':'PX-0001'}"></label>
        <label class="full">Ghi chú<textarea name="notes" maxlength="500" placeholder="Ghi chú nghiệp vụ..."></textarea></label>
        <div class="modal-actions full"><button type="button" class="btn btn-outline" data-close-modal>Hủy</button><button id="movementSubmit" class="btn ${isReceipt?'btn-success':'btn-warning'}" disabled>${isReceipt?'Xác nhận nhập kho':'Xác nhận xuất kho'}</button></div>
      </form>`);

    const form=$('#movementForm');
    const warehouseSelect=$('#movementWarehouse');
    const productSelect=$('#movementProduct');
    const quantityInput=$('#movementQuantity');
    const hint=$('#movementProductHint');
    const submitButton=$('#movementSubmit');
    let warehouseStocks=[];
    let stockByProduct=new Map();

    function updateMovementProductHint(){
      const productId=Number(productSelect.value);
      if(!productId){
        quantityInput.removeAttribute('max');
        submitButton.disabled=true;
        return;
      }
      const stock=stockByProduct.get(productId);
      if(isReceipt){
        hint.textContent=stock
          ? `Sản phẩm đã có tại kho này. Tồn hiện tại: ${number(stock.quantity)} ${stock.unit}. Nhập thêm sẽ cộng vào số lượng tồn.`
          : 'Sản phẩm chưa có tại kho này. Khi nhập, hệ thống sẽ tạo dòng tồn kho mới.';
        quantityInput.removeAttribute('max');
      }else{
        const available=Number(stock?.quantity||0);
        hint.textContent=`Số lượng hiện có tại kho: ${number(available)} ${stock?.unit||''}. Số lượng xuất không được vượt quá tồn hiện tại.`;
        quantityInput.max=String(available);
        if(Number(quantityInput.value)>available) quantityInput.value=available>0?String(available):'1';
      }
      submitButton.disabled=false;
    }

    async function refreshProductsForWarehouse(){
      const warehouseId=Number(warehouseSelect.value);
      productSelect.disabled=true;
      productSelect.innerHTML='<option value="">Đang tải sản phẩm...</option>';
      submitButton.disabled=true;
      hint.textContent='Đang tải tồn kho của kho đã chọn...';
      quantityInput.removeAttribute('max');
      if(!warehouseId){
        productSelect.innerHTML='<option value="">Vui lòng chọn kho trước</option>';
        hint.textContent=isReceipt
          ? 'Chọn kho để xem sản phẩm đã có và chưa có tại kho.'
          : 'Chọn kho để chỉ xem sản phẩm đang còn tồn tại kho đó.';
        return;
      }
      try{
        warehouseStocks=await get(`/api/inventory?warehouseId=${warehouseId}`);
        stockByProduct=new Map((warehouseStocks||[]).map(x=>[Number(x.productId),x]));
        let options='';
        if(isReceipt){
          const inWarehouse=allProducts.filter(p=>stockByProduct.has(Number(p.id)));
          const notInWarehouse=allProducts.filter(p=>!stockByProduct.has(Number(p.id)));
          if(inWarehouse.length){
            options+=`<optgroup label="Đang có tại kho (${inWarehouse.length})">${inWarehouse.map(p=>{
              const st=stockByProduct.get(Number(p.id));
              return `<option value="${p.id}">${esc(p.sku)} · ${esc(p.name)} — tồn ${number(st.quantity)} ${esc(st.unit)}</option>`;
            }).join('')}</optgroup>`;
          }
          if(notInWarehouse.length){
            options+=`<optgroup label="Chưa có tại kho — có thể nhập mới (${notInWarehouse.length})">${notInWarehouse.map(p=>`<option value="${p.id}">${esc(p.sku)} · ${esc(p.name)} — chưa có tồn</option>`).join('')}</optgroup>`;
          }
          hint.textContent='Danh sách được chia theo tình trạng tại kho đã chọn. Có thể nhập thêm hàng đã tồn hoặc nhập sản phẩm mới vào kho.';
        }else{
          const availableProducts=allProducts
            .filter(p=>stockByProduct.has(Number(p.id)) && Number(stockByProduct.get(Number(p.id)).quantity)>0)
            .sort((a,b)=>String(a.sku).localeCompare(String(b.sku),'vi'));
          options=availableProducts.map(p=>{
            const st=stockByProduct.get(Number(p.id));
            return `<option value="${p.id}">${esc(p.sku)} · ${esc(p.name)} — còn ${number(st.quantity)} ${esc(st.unit)}</option>`;
          }).join('');
          hint.textContent=availableProducts.length
            ? `Chỉ hiển thị ${availableProducts.length} sản phẩm đang có số lượng tồn lớn hơn 0 tại kho đã chọn.`
            : 'Kho này chưa có sản phẩm nào còn tồn để xuất.';
        }
        productSelect.innerHTML=`<option value="">-- Chọn sản phẩm --</option>${options}`;
        productSelect.disabled=!options;
        if(!options){
          productSelect.innerHTML='<option value="">Không có sản phẩm phù hợp tại kho này</option>';
          submitButton.disabled=true;
        }
      }catch(err){
        productSelect.innerHTML='<option value="">Không tải được danh sách sản phẩm</option>';
        hint.textContent=err.message;
        toast(err.message,'error');
      }
    }

    warehouseSelect.addEventListener('change',()=>refreshProductsForWarehouse());
    productSelect.addEventListener('change',updateMovementProductHint);
    quantityInput.addEventListener('input',()=>{
      if(!isReceipt){
        const stock=stockByProduct.get(Number(productSelect.value));
        const max=Number(stock?.quantity||0);
        if(max>0 && Number(quantityInput.value)>max){
          quantityInput.setCustomValidity(`Kho chỉ còn ${number(max)} ${stock.unit}.`);
        }else quantityInput.setCustomValidity('');
      }
    });

    form.addEventListener('submit',async e=>{
      e.preventDefault();
      const f=new FormData(form);
      const productId=Number(f.get('productId'));
      const warehouseId=Number(f.get('warehouseId'));
      const quantity=Number(f.get('quantity'));
      if(!warehouseId||!productId){toast('Vui lòng chọn kho và sản phẩm.','error');return;}
      const currentStock=stockByProduct.get(productId);
      if(!isReceipt && (!currentStock || quantity>Number(currentStock.quantity))){
        toast(`Số lượng xuất vượt tồn kho. Tồn hiện tại: ${number(currentStock?.quantity||0)} ${currentStock?.unit||''}.`,'error');
        return;
      }
      const body={productId,warehouseId,quantity,referenceNo:f.get('referenceNo')||null,notes:f.get('notes')||null};
      try{
        submitButton.disabled=true;
        const result=await post(`/api/inventory/${isReceipt?'receipts':'issues'}`,body);
        closeModal();
        toast(result.message||(isReceipt?'Nhập kho thành công.':'Xuất kho thành công.'));
        await loadInventory();
        await loadDashboard();
        if(state.page==='movements') await loadMovements();
      }catch(err){
        toast(err.message,'error');
        submitButton.disabled=false;
      }
    });
  }
  async function loadMovements(){const result=await get(`/api/inventory/movements?pageNumber=${state.movementPage}&pageSize=${state.movementPageSize}`);const items=result.items||[];$('#movementsTable').innerHTML=items.length?`<table><thead><tr><th>THỜI GIAN</th><th>LOẠI</th><th>SẢN PHẨM</th><th>KHO</th><th>SỐ LƯỢNG</th><th>MÃ THAM CHIẾU</th><th>NGƯỜI THỰC HIỆN</th></tr></thead><tbody>${items.map(x=>`<tr><td>${date(x.createdAt)}</td><td><span class="pill ${x.movementType==='IMPORT'?'pill-green':'pill-amber'}">${x.movementType==='IMPORT'?'NHẬP':'XUẤT'}</span></td><td><div class="table-primary">${esc(x.productName)}</div><div class="table-secondary">${esc(x.sku)}</div></td><td>${esc(x.warehouseName)}</td><td><b>${number(x.quantity)}</b></td><td>${esc(x.referenceNo||'—')}</td><td>${esc(x.createdBy||'—')}</td></tr>`).join('')}</tbody></table>`:tableEmpty('Chưa có giao dịch.');pager('#movementsPager',result.pageNumber,result.pageSize,result.totalCount,p=>{state.movementPage=p;loadMovements().catch(e=>toast(e.message,'error'));});}
  function openModal(title,body){$('#modalHost').innerHTML=`<div class="modal-backdrop" id="modalBackdrop"><div class="modal"><div class="modal-header"><h3>${esc(title)}</h3><button class="icon-btn" data-close-modal aria-label="Đóng">✕</button></div><div class="modal-body">${body}</div></div></div>`;$$('[data-close-modal]').forEach(b=>b.addEventListener('click',closeModal));$('#modalBackdrop').addEventListener('click',e=>{if(e.target.id==='modalBackdrop')closeModal();});}
  function closeModal(){$('#modalHost').innerHTML='';}

  $('#loginForm').addEventListener('submit',async e=>{e.preventDefault();const f=new FormData(e.currentTarget);const btn=$('button[type="submit"]',e.currentTarget);btn.disabled=true;btn.textContent='Đang đăng nhập...';try{const result=await api('/api/auth/login',{method:'POST',body:JSON.stringify({usernameOrEmail:f.get('usernameOrEmail'),password:f.get('password')})});state.token=result.token;state.user=result.user;sessionStorage.setItem('wm_token',state.token);$('#loginError').textContent='';showApp();}catch(err){$('#loginError').textContent=err.message;}finally{btn.disabled=false;btn.innerHTML='Đăng nhập <span>→</span>';}});
  $('#logoutBtn').addEventListener('click',()=>{state.token='';state.user=null;sessionStorage.removeItem('wm_token');showLogin();toast('Bạn đã đăng xuất.');});
  $$('.nav-link').forEach(b=>b.addEventListener('click',()=>navigate(b.dataset.page)));
  $$('[data-go]').forEach(b=>b.addEventListener('click',()=>navigate(b.dataset.go,{action:b.dataset.action})));
  $('#addProductBtn').addEventListener('click',()=>openProductModal());$('#searchProducts').addEventListener('click',()=>{state.productPage=1;loadProducts().catch(e=>toast(e.message,'error'));});
  $('#productSearch').addEventListener('keydown',e=>{if(e.key==='Enter'){state.productPage=1;loadProducts().catch(err=>toast(err.message,'error'));}});
  $('#productCategory').addEventListener('change',()=>{state.productPage=1;loadProducts().catch(e=>toast(e.message,'error'));});$('#productSort').addEventListener('change',()=>loadProducts().catch(e=>toast(e.message,'error')));$('#productDirection').addEventListener('change',()=>loadProducts().catch(e=>toast(e.message,'error')));
  $('#addCategoryBtn').addEventListener('click',()=>openCategoryModal());$('#addSupplierBtn').addEventListener('click',()=>openSupplierModal());$('#addWarehouseBtn').addEventListener('click',()=>openWarehouseModal());
  $('#refreshDashboard').addEventListener('click',()=>loadDashboard().then(()=>toast('Đã cập nhật tổng quan.')).catch(e=>toast(e.message,'error')));
  $('#refreshInventory').addEventListener('click',()=>loadInventory().catch(e=>toast(e.message,'error')));$('#stockWarehouse').addEventListener('change',()=>loadInventory().catch(e=>toast(e.message,'error')));
  $('#openReceipt').addEventListener('click',()=>openMovementModal('receipt').catch(e=>toast(e.message,'error')));$('#openIssue').addEventListener('click',()=>openMovementModal('issue').catch(e=>toast(e.message,'error')));
  $('#refreshMovements').addEventListener('click',()=>loadMovements().catch(e=>toast(e.message,'error')));$('#movementPageSize').addEventListener('change',()=>{state.movementPage=1;state.movementPageSize=Number($('#movementPageSize').value);loadMovements().catch(e=>toast(e.message,'error'));});
  $('#menuBtn').addEventListener('click',()=>$('#sidebar').classList.toggle('open'));

  async function init(){if(!state.token){showLogin();return;}try{state.user=await get('/api/auth/me');showApp();}catch{state.token='';sessionStorage.removeItem('wm_token');showLogin();}}
  init();
})();
