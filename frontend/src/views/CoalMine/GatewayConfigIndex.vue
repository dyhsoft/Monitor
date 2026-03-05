<template>
  <div>
    <BasicTable :tableConfig="tableConfig" :searchForm="searchForm" :data="dataList" :loading="loading">
      <template #toolbar>
        <a-button type="primary" @click="handleAdd">
          <template #icon><PlusOutlined /></template>
          新增网关
        </a-button>
      </template>
      
      <template #columns>
        <a-table-column title="网关编号" data-index="gatewayCode" key="gatewayCode" />
        <a-table-column title="网关名称" data-index="gatewayName" key="gatewayName" />
        <a-table-column title="所属煤矿" data-index="mineName" key="mineName" />
        <a-table-column title="网关类型" data-index="gatewayType" key="gatewayType">
          <template #cell="{ record }">
            <a-tag :color="getGatewayTypeColor(record.gatewayType)">
              {{ record.gatewayType }}
            </a-tag>
          </template>
        </a-table-column>
        <a-table-column title="FTP地址" data-index="ftpHost" key="ftpHost" />
        <a-table-column title="FTP端口" data-index="ftpPort" key="ftpPort" />
        <a-table-column title="文件编码" data-index="fileEncoding" key="fileEncoding" />
        <a-table-column title="推送频率(秒)" data-index="pushFrequency" key="pushFrequency" />
        <a-table-column title="状态" data-index="status" key="status">
          <template #cell="{ record }">
            <a-tag :color="record.status === 1 ? 'green' : 'red'">
              {{ record.status === 1 ? '启用' : '停用' }}
            </a-tag>
          </template>
        </a-table-column>
        <a-table-column title="操作" key="action" width="180">
          <template #cell="{ record }">
            <a-space>
              <a-button type="link" size="small" @click="handleEdit(record)">编辑</a-button>
              <a-button type="link" size="small" @click="handleTest(record)">测试连接</a-button>
              <a-button type="link" size="small" danger @click="handleDelete(record)">删除</a-button>
            </a-space>
          </template>
        </a-table-column>
      </template>
    </BasicTable>

    <!-- 新增/编辑弹窗 -->
    <a-modal
      v-model:open="modalVisible"
      :title="modalTitle"
      width="800px"
      @ok="handleSubmit"
      @cancel="modalVisible = false"
    >
      <a-form
        ref="formRef"
        :model="formData"
        :label-col="{ span: 6 }"
        :wrapper-col="{ span: 16 }"
      >
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="所属煤矿" name="mineId" :rules="[{ required: true, message: '请选择煤矿' }]">
              <a-select v-model:value="formData.mineId" placeholder="请选择煤矿" show-search :filter-option="filterOption">
                <a-select-option v-for="item in mineList" :key="item.id" :value="item.id">
                  {{ item.name }}
                </a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="网关类型" name="gatewayType" :rules="[{ required: true, message: '请选择网关类型' }]">
              <a-select v-model:value="formData.gatewayType" placeholder="请选择">
                <a-select-option value="安全监测">安全监测</a-select-option>
                <a-select-option value="人员定位">人员定位</a-select-option>
                <a-select-option value="水害防治">水害防治</a-select-option>
                <a-select-option value="视频监控">视频监控</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
        </a-row>
        
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="网关编号" name="gatewayCode" :rules="[{ required: true, message: '请输入网关编号' }]">
              <a-input v-model:value="formData.gatewayCode" placeholder="网关编号" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="网关名称" name="gatewayName">
              <a-input v-model:value="formData.gatewayName" placeholder="网关名称" />
            </a-form-item>
          </a-col>
        </a-row>
        
        <a-divider>FTP配置</a-divider>
        
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="FTP主机" name="ftpHost">
              <a-input v-model:value="formData.ftpHost" placeholder="IP地址或域名" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="FTP端口" name="ftpPort">
              <a-input-number v-model:value="formData.ftpPort" :min="1" :max="65535" style="width: 100%" />
            </a-form-item>
          </a-col>
        </a-row>
        
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="FTP用户名" name="ftpUsername">
              <a-input v-model:value="formData.ftpUsername" placeholder="FTP用户名" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="FTP密码" name="ftpPassword">
              <a-input-password v-model:value="formData.ftpPassword" placeholder="FTP密码" />
            </a-form-item>
          </a-col>
        </a-row>
        
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="FTP根目录" name="ftpRootPath">
              <a-input v-model:value="formData.ftpRootPath" placeholder="/或/data" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="数据目录" name="dataPath">
              <a-input v-model:value="formData.dataPath" placeholder="数据目录" />
            </a-form-item>
          </a-col>
        </a-row>
        
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="文件编码" name="fileEncoding">
              <a-select v-model:value="formData.fileEncoding" placeholder="文件编码">
                <a-select-option value="UTF-8">UTF-8</a-select-option>
                <a-select-option value="GBK">GBK</a-select-option>
                <a-select-option value="GB2312">GB2312</a-select-option>
                <a-select-option value="GB18030">GB18030</a-select-option>
              </a-select>
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="推送频率(秒)" name="pushFrequency">
              <a-input-number v-model:value="formData.pushFrequency" :min="10" :max="3600" style="width: 100%" />
            </a-form-item>
          </a-col>
        </a-row>
        
        <a-row :gutter="16">
          <a-col :span="12">
            <a-form-item label="IP限制" name="allowedIp">
              <a-input v-model:value="formData.allowedIp" placeholder="允许的IP地址，多个用逗号分隔" />
            </a-form-item>
          </a-col>
          <a-col :span="12">
            <a-form-item label="状态" name="status">
              <a-switch v-model:checked="formData.status" checked-value="1" unchecked-value="0" />
            </a-form-item>
          </a-col>
        </a-row>
        
        <a-form-item label="备注" name="remark">
          <a-textarea v-model:value="formData.remark" placeholder="备注" :rows="2" />
        </a-form-item>
      </a-form>
    </a-modal>
  </div>
</template>

<script setup>
import { ref, reactive } from 'vue';
import { message } from 'ant-design-vue';
import { BasicTable } from '@/components/Table';
import { PlusOutlined } from '@ant-design/icons-vue';

const loading = ref(false);
const dataList = ref([]);
const mineList = ref([]);
const modalVisible = ref(false);
const modalTitle = ref('新增网关');
const formRef = ref(null);

const searchForm = reactive({
  mineId: null,
  gatewayCode: '',
  gatewayType: '',
  status: null
});

const formData = reactive({
  id: null,
  mineId: null,
  gatewayCode: '',
  gatewayName: '',
  gatewayType: '',
  ftpHost: '',
  ftpPort: 21,
  ftpUsername: '',
  ftpPassword: '',
  ftpRootPath: '',
  dataPath: '',
  fileEncoding: 'UTF-8',
  pushFrequency: 60,
  allowedIp: '',
  status: '1',
  remark: ''
});

const tableConfig = reactive({
  api: '/api/CoalMine/GatewayConfig/page',
  method: 'POST',
  columns: [],
  pagination: true,
  showIndex: true
});

const getGatewayTypeColor = (type) => {
  const colorMap = {
    '安全监测': 'blue',
    '人员定位': 'green',
    '水害防治': 'cyan',
    '视频监控': 'purple'
  };
  return colorMap[type] || 'default';
};

const filterOption = (input, option) => {
  return option.value.toLowerCase().includes(input.toLowerCase());
};

// 加载煤矿列表
const loadMineList = async () => {
  // TODO: 调用API获取煤矿列表
  mineList.value = [];
};

loadMineList();
</script>
