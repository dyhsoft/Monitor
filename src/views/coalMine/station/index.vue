<template>
  <div class="station-container">
    <el-form :inline="true" :model="queryParams" class="search-form">
      <el-form-item label="煤矿">
        <el-select v-model="queryParams.mineId" placeholder="请选择煤矿" clearable @change="handleQuery">
          <el-option v-for="item in mineList" :key="item.value" :label="item.label" :value="item.value"></el-option>
        </el-select>
      </el-form-item>
      <el-form-item label="基站编号">
        <el-input v-model="queryParams.stationCode" placeholder="请输入基站编号" clearable @keyup.enter="handleQuery" />
      </el-form-item>
      <el-form-item label="状态">
        <el-select v-model="queryParams.status" placeholder="请选择状态" clearable @change="handleQuery">
          <el-option label="在线" :value="1" />
          <el-option label="离线" :value="0" />
        </el-select>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="handleQuery">查询</el-button>
        <el-button @click="resetQuery">重置</el-button>
        <el-button type="primary" @click="handleAdd">新增</el-button>
      </el-form-item>
    </el-form>

    <el-table v-loading="loading" :data="stationList" border stripe>
      <el-table-column label="序号" type="index" width="60" align="center" />
      <el-table-column label="煤矿" prop="mineName" align="center" />
      <el-table-column label="基站编号" prop="stationCode" align="center" />
      <el-table-column label="基站名称" prop="stationName" align="center" />
      <el-table-column label="位置" prop="location" align="center" />
      <el-table-column label="状态" prop="status" align="center">
        <template #default="{ row }">
          <el-tag :type="row.status === 1 ? 'success' : 'danger'">
            {{ row.status === 1 ? '在线' : '离线' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="最后更新时间" prop="updateTime" align="center" width="180" />
      <el-table-column label="操作" width="180" align="center">
        <template #default="{ row }">
          <el-button type="primary" link @click="handleEdit(row)">编辑</el-button>
          <el-button type="danger" link @click="handleDelete(row)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-pagination
      v-model:current-page="queryParams.page"
      v-model:page-size="queryParams.pageSize"
      :page-sizes="[10, 20, 50, 100]"
      :total="total"
      layout="total, sizes, prev, pager, next, jumper"
      @size-change="getList"
      @current-change="getList"
    />

    <el-dialog v-model="dialogVisible" :title="isEdit ? '编辑基站' : '新增基站'" width="600px">
      <el-form ref="formRef" :model="form" :rules="rules" label-width="100px">
        <el-form-item label="煤矿" prop="mineId">
          <el-select v-model="form.mineId" placeholder="请选择煤矿">
            <el-option v-for="item in mineList" :key="item.value" :label="item.label" :value="item.value"></el-option>
          </el-select>
        </el-form-item>
        <el-form-item label="基站编号" prop="stationCode">
          <el-input v-model="form.stationCode" placeholder="请输入基站编号" />
        </el-form-item>
        <el-form-item label="基站名称" prop="stationName">
          <el-input v-model="form.stationName" placeholder="请输入基站名称" />
        </el-form-item>
        <el-form-item label="位置" prop="location">
          <el-input v-model="form.location" placeholder="请输入位置" />
        </el-form-item>
        <el-form-item label="备注" prop="remark">
          <el-input v-model="form.remark" type="textarea" :rows="2" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="submitForm" :loading="submitLoading">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { getAPI } from '/@/utils/axios-utils';
import { CoalMineApi, StationApi } from '/@/api-services/api';

const loading = ref(false)
const total = ref(0)
const mineList = ref<any[]>([])
const stationList = ref<any[]>([])
const dialogVisible = ref(false)
const isEdit = ref(false)
const submitLoading = ref(false)
const formRef = ref()

const queryParams = reactive({
  page: 1,
  pageSize: 10,
  mineId: null as number | null,
  stationCode: '',
  status: null as number | null
})

const form = reactive<any>({
  mineId: null,
  stationCode: '',
  stationName: '',
  location: '',
  remark: ''
})

const rules = {
  mineId: [{ required: true, message: '请选择煤矿', trigger: 'change' }],
  stationCode: [{ required: true, message: '请输入基站编号', trigger: 'blur' }],
  stationName: [{ required: true, message: '请输入基站名称', trigger: 'blur' }]
}

const getMineList = async () => {
  try {
    const res = await getAPI(CoalMineApi).getList({ page: 1, pageSize: 1000 })
    mineList.value = (res.data.result || []).map((item: any) => ({ label: item.name, value: item.id }))
  } catch (error) {
    console.error('获取煤矿列表失败:', error)
  }
}

const getList = async () => {
  loading.value = true
  try {
    const res = await getAPI(StationApi).getPage(queryParams)
    stationList.value = res.data.result?.rows || res.data.result || []
    total.value = res.data.result?.total || 0
  } catch (error) {
    console.error('获取基站列表失败:', error)
  } finally {
    loading.value = false
  }
}

const handleQuery = () => {
  queryParams.page = 1
  getList()
}

const resetQuery = () => {
  queryParams.mineId = null
  queryParams.stationCode = ''
  queryParams.status = null
  handleQuery()
}

const handleAdd = () => {
  Object.assign(form, { mineId: null, stationCode: '', stationName: '', location: '', remark: '' })
  isEdit.value = false
  dialogVisible.value = true
}

const handleEdit = (row: any) => {
  Object.assign(form, row)
  isEdit.value = true
  dialogVisible.value = true
}

const handleDelete = async (row: any) => {
  try {
    await ElMessageBox.confirm('确认删除该基站吗？', '提示', { type: 'warning' })
    await getAPI(StationApi).delete(row.id)
    ElMessage.success('删除成功')
    getList()
  } catch (error) {
    console.error('删除失败:', error)
  }
}

const submitForm = async () => {
  if (!formRef.value) return
  await formRef.value.validate()
  submitLoading.value = true
  try {
    if (isEdit.value) {
      await getAPI(StationApi).update(form)
    } else {
      await getAPI(StationApi).add(form)
    }
    ElMessage.success(isEdit.value ? '编辑成功' : '新增成功')
    dialogVisible.value = false
    getList()
  } catch (error) {
    console.error('操作失败:', error)
  } finally {
    submitLoading.value = false
  }
}

onMounted(async () => {
  await getMineList()
  getList()
})
</script>

<style scoped>
.station-container {
  padding: 16px;
}
.search-form {
  margin-bottom: 16px;
}
</style>
