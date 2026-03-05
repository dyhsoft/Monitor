<template>
  <div class="video-container">
    <!-- 搜索表单 -->
    <div class="search-form">
      <el-form :inline="true" :model="searchForm" class="demo-form-inline">
        <el-form-item label="煤矿">
          <el-select v-model="searchForm.mineId" placeholder="请选择煤矿" clearable filterable>
            <el-option v-for="item in mineList" :key="item.id" :label="item.name" :value="item.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="设备编号">
          <el-input v-model="searchForm.deviceCode" placeholder="请输入设备编号" clearable />
        </el-form-item>
        <el-form-item label="设备名称">
          <el-input v-model="searchForm.deviceName" placeholder="请输入设备名称" clearable />
        </el-form-item>
        <el-form-item label="设备类型">
          <el-select v-model="searchForm.deviceType" placeholder="请选择设备类型" clearable>
            <el-option v-for="item in deviceTypes" :key="item.code" :label="item.name" :value="item.code" />
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="loadData">查询</el-button>
          <el-button @click="resetSearch">重置</el-button>
        </el-form-item>
      </el-form>
    </div>

    <!-- 操作按钮 -->
    <div class="toolbar">
      <el-button type="primary" @click="handleAdd">
        <el-icon><Plus /></el-icon>新增设备
      </el-button>
      <el-button type="danger" :disabled="selectedRows.length === 0" @click="handleBatchDelete">
        <el-icon><Delete /></el-icon>批量删除
      </el-button>
    </div>

    <!-- 数据表格 -->
    <el-table :data="tableData" v-loading="loading" @selection-change="handleSelectionChange" border>
      <el-table-column type="selection" width="50" align="center" />
      <el-table-column prop="mineName" label="煤矿名称" width="150" align="center" />
      <el-table-column prop="deviceCode" label="设备编号" width="120" align="center" />
      <el-table-column prop="deviceName" label="设备名称" width="150" align="center" />
      <el-table-column prop="deviceType" label="设备类型" width="100" align="center">
        <template #default="{ row }">
          <el-tag>{{ getDeviceTypeName(row.deviceType) }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="ipAddress" label="IP地址" width="130" align="center" />
      <el-table-column prop="port" label="端口" width="80" align="center" />
      <el-table-column prop="channel" label="通道号" width="80" align="center" />
      <el-table-column prop="streamType" label="码流" width="80" align="center">
        <template #default="{ row }">
          <el-tag :type="row.streamType === 1 ? 'success' : 'info'">
            {{ row.streamType === 1 ? '主码流' : '子码流' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="status" label="状态" width="80" align="center">
        <template #default="{ row }">
          <el-tag :type="row.status === 1 ? 'success' : 'danger'">
            {{ row.status === 1 ? '在线' : '离线' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="installLocation" label="安装位置" min-width="150" align="center" />
      <el-table-column prop="createTime" label="创建时间" width="170" align="center" />
      <el-table-column label="操作" width="200" align="center" fixed="right">
        <template #default="{ row }">
          <el-button link type="primary" @click="handleEdit(row)">编辑</el-button>
          <el-button link type="success" @click="handlePlay(row)">播放</el-button>
          <el-button link type="danger" @click="handleDelete(row)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>

    <!-- 分页 -->
    <el-pagination
      v-model:current-page="pagination.current"
      v-model:page-size="pagination.size"
      :page-sizes="[10, 20, 50, 100]"
      :total="pagination.total"
      layout="total, sizes, prev, pager, next, jumper"
      @size-change="loadData"
      @current-change="loadData"
    />

    <!-- 新增/编辑对话框 -->
    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="700px" destroy-on-close>
      <el-form ref="formRef" :model="formData" :rules="formRules" label-width="100px">
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="煤矿" prop="mineId">
              <el-select v-model="formData.mineId" placeholder="请选择煤矿" filterable>
                <el-option v-for="item in mineList" :key="item.id" :label="item.name" :value="item.id" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="设备编号" prop="deviceCode">
              <el-input v-model="formData.deviceCode" placeholder="请输入设备编号" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="设备名称" prop="deviceName">
              <el-input v-model="formData.deviceName" placeholder="请输入设备名称" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="设备类型" prop="deviceType">
              <el-select v-model="formData.deviceType" placeholder="请选择设备类型">
                <el-option v-for="item in deviceTypes" :key="item.code" :label="item.name" :value="item.code" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="IP地址" prop="ipAddress">
              <el-input v-model="formData.ipAddress" placeholder="如: 192.168.1.100" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="端口" prop="port">
              <el-input-number v-model="formData.port" :min="1" :max="65535" style="width: 100%" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="通道号" prop="channel">
              <el-input-number v-model="formData.channel" :min="1" :max="256" style="width: 100%" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="码流类型" prop="streamType">
              <el-select v-model="formData.streamType" placeholder="请选择码流类型">
                <el-option label="主码流" :value="1" />
                <el-option label="子码流" :value="2" />
              </el-select>
            </el-form-item>
          </el-col>
        </el-row>
        <el-row :gutter="20">
          <el-col :span="12">
            <el-form-item label="用户名" prop="username">
              <el-input v-model="formData.username" placeholder="请输入用户名" />
            </el-form-item>
          </el-col>
          <el-col :span="12">
            <el-form-item label="密码" prop="password">
              <el-input v-model="formData.password" type="password" show-password placeholder="请输入密码" />
            </el-form-item>
          </el-col>
        </el-row>
        <el-form-item label="安装位置" prop="installLocation">
          <el-input v-model="formData.installLocation" placeholder="请输入安装位置" />
        </el-form-item>
        <el-form-item label="备注" prop="remark">
          <el-input v-model="formData.remark" type="textarea" :rows="2" placeholder="请输入备注" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="handleSubmit">确定</el-button>
        <el-button type="success" @click="handleTestConnection">测试连接</el-button>
      </template>
    </el-dialog>

    <!-- 视频播放对话框 -->
    <el-dialog v-model="playDialogVisible" title="视频播放" width="900px" destroy-on-close>
      <div class="video-player">
        <video v-if="playUrl" id="videoPlayer" class="video-js vjs-default-skin" controls preload="auto">
          <source :src="playUrl" type="application/x-mpegURL" />
        </video>
        <div v-else class="no-video">正在加载视频...</div>
      </div>
    </el-dialog>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Plus, Delete } from '@element-plus/icons-vue'
import axios from 'axios'

// 搜索表单
const searchForm = reactive({
  mineId: null,
  deviceCode: '',
  deviceName: '',
  deviceType: ''
})

// 分页数据
const pagination = reactive({
  current: 1,
  size: 10,
  total: 0
})

// 表格数据
const tableData = ref([])
const loading = ref(false)
const selectedRows = ref([])

// 煤矿列表
const mineList = ref<any[]>([])

// 设备类型
const deviceTypes = ref<any[]>([
  { code: 'HIKVISION', name: '海康威视' },
  { code: 'DH', name: '大华' },
  { code: 'UNIVIEW', name: '宇视' },
  { code: 'ONVIF', name: 'ONVIF' },
  { code: 'RTSP', name: 'RTSP' }
])

// 对话框
const dialogVisible = ref(false)
const dialogTitle = ref('')
const formRef = ref()
const formData = reactive({
  id: 0,
  mineId: null,
  deviceCode: '',
  deviceName: '',
  deviceType: 'HIKVISION',
  ipAddress: '',
  port: 8000,
  channel: 1,
  streamType: 1,
  username: '',
  password: '',
  installLocation: '',
  remark: ''
})

// 表单校验规则
const formRules = {
  mineId: [{ required: true, message: '请选择煤矿', trigger: 'change' }],
  deviceCode: [{ required: true, message: '请输入设备编号', trigger: 'blur' }],
  deviceName: [{ required: true, message: '请输入设备名称', trigger: 'blur' }],
  deviceType: [{ required: true, message: '请选择设备类型', trigger: 'change' }],
  ipAddress: [
    { required: true, message: '请输入IP地址', trigger: 'blur' },
    { pattern: /^(\d{1,3}\.){3}\d{1,3}$/, message: '请输入正确的IP地址', trigger: 'blur' }
  ],
  port: [{ required: true, message: '请输入端口', trigger: 'blur' }],
  username: [{ required: true, message: '请输入用户名', trigger: 'blur' }],
  password: [{ required: true, message: '请输入密码', trigger: 'blur' }]
}

// 视频播放
const playDialogVisible = ref(false)
const playUrl = ref('')

// 加载数据
const loadData = async () => {
  loading.value = true
  try {
    const params = {
      ...searchForm,
      current: pagination.current,
      size: pagination.size
    }
    const res = await axios.post('/api/services/app/Video/GetPage', params)
    tableData.value = res.data.items || []
    pagination.total = res.data.totalCount || 0
  } catch (error: any) {
    ElMessage.error(error.message || '加载失败')
  } finally {
    loading.value = false
  }
}

// 加载煤矿列表
const loadMineList = async () => {
  try {
    const res = await axios.get('/api/services/app/CoalMine/All')
    mineList.value = res.data || []
  } catch (error: any) {
    console.error('加载煤矿列表失败:', error)
  }
}

// 重置搜索
const resetSearch = () => {
  searchForm.mineId = null
  searchForm.deviceCode = ''
  searchForm.deviceName = ''
  searchForm.deviceType = ''
  pagination.current = 1
  loadData()
}

// 新增
const handleAdd = () => {
  dialogTitle.value = '新增视频设备'
  Object.assign(formData, {
    id: 0,
    mineId: null,
    deviceCode: '',
    deviceName: '',
    deviceType: 'HIKVISION',
    ipAddress: '',
    port: 8000,
    channel: 1,
    streamType: 1,
    username: '',
    password: '',
    installLocation: '',
    remark: ''
  })
  dialogVisible.value = true
}

// 编辑
const handleEdit = async (row: any) => {
  dialogTitle.value = '编辑视频设备'
  const res = await axios.get(`/api/services/app/Video/Get`, { params: { id: row.id } })
  Object.assign(formData, res.data)
  dialogVisible.value = true
}

// 删除
const handleDelete = (row: any) => {
  ElMessageBox.confirm('确定要删除该视频设备吗？', '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    try {
      await axios.delete(`/api/services/app/Video/Delete`, { params: { id: row.id } })
      ElMessage.success('删除成功')
      loadData()
    } catch (error: any) {
      ElMessage.error(error.message || '删除失败')
    }
  })
}

// 批量删除
const handleBatchDelete = () => {
  if (selectedRows.value.length === 0) return
  const ids = selectedRows.value.map((row: any) => row.id).join(',')
  ElMessageBox.confirm(`确定要删除选中的 ${selectedRows.value.length} 条记录吗？`, '提示', {
    confirmButtonText: '确定',
    cancelButtonText: '取消',
    type: 'warning'
  }).then(async () => {
    // 批量删除逻辑
    for (const row of selectedRows.value) {
      await axios.delete(`/api/services/app/Video/Delete`, { params: { id: row.id } })
    }
    ElMessage.success('批量删除成功')
    loadData()
  })
}

// 提交表单
const handleSubmit = async () => {
  const valid = await formRef.value?.validate()
  if (!valid) return

  try {
    if (formData.id === 0) {
      await axios.post('/api/services/app/Video/Add', formData)
      ElMessage.success('新增成功')
    } else {
      await axios.put('/api/services/app/Video/Update', formData)
      ElMessage.success('更新成功')
    }
    dialogVisible.value = false
    loadData()
  } catch (error: any) {
    ElMessage.error(error.message || '操作失败')
  }
}

// 测试连接
const handleTestConnection = async () => {
  try {
    const res = await axios.post('/api/services/app/Video/TestConnection', formData)
    if (res.data) {
      ElMessage.success('连接成功')
    } else {
      ElMessage.error('连接失败')
    }
  } catch (error: any) {
    ElMessage.error(error.message || '测试失败')
  }
}

// 播放视频
const handlePlay = async (row: any) => {
  try {
    const res = await axios.get('/api/services/app/Video/GetPlayUrl', { params: { id: row.id } })
    playUrl.value = res.data
    playDialogVisible.value = true
  } catch (error: any) {
    ElMessage.error(error.message || '获取播放地址失败')
  }
}

// 选择行
const handleSelectionChange = (rows: any[]) => {
  selectedRows.value = rows
}

// 获取设备类型名称
const getDeviceTypeName = (code: string) => {
  const item = deviceTypes.value.find(t => t.code === code)
  return item ? item.name : code
}

onMounted(() => {
  loadMineList()
  loadData()
})
</script>

<style scoped>
.video-container {
  padding: 20px;
}

.search-form {
  margin-bottom: 15px;
}

.toolbar {
  margin-bottom: 15px;
}

.el-pagination {
  margin-top: 15px;
  justify-content: flex-end;
}

.video-player {
  width: 100%;
  height: 500px;
  background: #000;
  display: flex;
  align-items: center;
  justify-content: center;
}

.video-player video {
  width: 100%;
  height: 100%;
}

.no-video {
  color: #fff;
  font-size: 16px;
}
</style>
