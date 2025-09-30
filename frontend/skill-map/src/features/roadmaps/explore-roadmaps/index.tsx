'use client';

import { useState } from 'react';
import { useGetRoadmapsQuery } from '../api';
import RoadmapGrid from '@/components/roadmap/roadmapGrid';
import SpinnerScreen from '@/components/base/spinner';
import SearchContainer from '@/components/search-container';
import ErrorScreen from '@/components/base/error';
import { defaultPagination } from '../helpers';

// // todo: implement search and filtering
// export default function ExploreRoadmapsPage() {
//   const { pageSize: deafultPageSize, pageNumber: defaultPageNumber } =
//     defaultPagination;
//   const [page, setPage] = useState(defaultPageNumber);

//   const { data, error, isLoading, isFetching } = useGetRoadmapsQuery({
//     pageNumber: page,
//     pageSize: deafultPageSize,
//   });

//   const setPageSafe = (newPage: number) => {
//     if (newPage < 1) return 1;
//     setPage(newPage);
//     return newPage;
//   };

//   const roadmaps = data?.roadmaps ?? [];
//   if (error) {
//     return <ErrorScreen />;
//   }

//   if (isLoading) {
//     return <SpinnerScreen />;
//   }

//   return (
//     <SearchContainer
//       disabled={isFetching}
//       page={page}
//       setPage={setPageSafe}
//       pageSize={deafultPageSize}
//       total={data?.total || 0}
//     >
//       <RoadmapGrid roadmaps={roadmaps} />
//     </SearchContainer>
//   );
// }
